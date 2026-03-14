using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MIPS.DeviceLib;
using MIPS.Net.IO;

namespace NetPC.Controllers
{
    internal class StorageCmd
    {
        public const byte FETCH_DIR_LIST = 0xFD;
        public const byte FETCH_FILES_LIST = 0xFF;
        public const byte IDLE = 0x0F;
        public const byte F_READ_ALL = 0xF0;
        public const byte F_OPEN = 0xF1;
        public const byte F_CLOSE = 0xF2;
        public const byte F_READ_BLOCK = 0xF3;
        public const byte F_WRITE_BLOCK = 0xF4;
        public const byte F_SET_POSITION = 0xF5;
        public const byte F_FLUSH = 0xF6;
        public const byte F_LENGTH = 0xF7;
    }


    internal class StorageController
    {
        private static MDHardware PORT;
        public static string PingSpeed { get; internal set; }

        static string root_dir = @"C:\mOS\";

        private static List<FileChunking> chunked_files = new List<FileChunking>();
        private static Dictionary<int, OpenFileDescriptor> open_files = new Dictionary<int, OpenFileDescriptor>();

        public static void INIT(int port)
        {
            if (PORT == null) PORT = new MDHardware();
            else
            {
                PORT.Disconnect();
                PORT = new MDHardware();
            }

            Connect(port);
        }

        private static void Connect(int port)
        {
            PORT.OnDataReceived += Hw_OnDataReceived;
            PORT.OnReceiveFailed += Hw_OnReceiveFailed;
            PORT.Connect(port, DeviceClass.STORAGE);
        }

        private static void Hw_OnReceiveFailed(string msg, bool connected)
        {
            if (!connected)
                return;
        }

        private static void Hw_OnDataReceived(byte[] data, int readed)
        {
            byte[] header = data[0..10];
            byte[] raw = new byte[data.Length - 10];
            Array.Copy(data, 10, raw, 0, raw.Length);

            byte operation = header[4];
            byte mode = header[5];

            if (operation != 0x00) return;

            switch (mode)
            {
                case StorageCmd.F_READ_ALL: ReadAllChunked(raw, readed - 10); break;
                case StorageCmd.FETCH_DIR_LIST: ListDirectories(raw, readed - 10); break;
                case StorageCmd.F_OPEN: Open(raw); break;
                case StorageCmd.F_CLOSE: Close(raw); break;
                case StorageCmd.F_READ_BLOCK: ReadBlock(raw); break;
                case StorageCmd.F_WRITE_BLOCK: WriteBlock(raw); break;
                case StorageCmd.F_LENGTH: Length(raw); break;
                case StorageCmd.F_FLUSH: Flush(raw); break;
                case StorageCmd.F_SET_POSITION: SetPosition(raw); break;
            }
        }

        // =========================
        // EXISTENTES (INALTERADOS)
        // =========================

        private static void ListDirectories(byte[] raw, int len)
        {
            string dir = Encoding.UTF8.GetString(raw, 0, len).TrimEnd('\0');
            string full = root_dir + dir.Replace("/", "\\");

            using MemoryStream ms = new MemoryStream(raw);
            using MemoryStream data = new MemoryStream();

            foreach (var di in new DirectoryInfo(full).GetDirectories())
            {
                data.Write(BitConverter.GetBytes(di.CreationTime.Ticks));
                data.Write(BitConverter.GetBytes(di.LastWriteTime.Ticks));
                data.Write(BitConverter.GetBytes(di.LastAccessTime.Ticks));
                data.Write(BitConverter.GetBytes(di.GetFiles().Length));
                data.Write(BitConverter.GetBytes(di.GetFiles().Sum(f => f.Length)));
                data.Write(Encoding.ASCII.GetBytes(di.Name));
                data.WriteByte(0);
            }

            ms.Write(BitConverter.GetBytes((int)data.Length));
            data.Position = 0;
            data.CopyTo(ms);

            PORT.SendResponse(raw);
        }

        private static void ReadAllChunked(byte[] raw, int len)
        {
            byte[] fNameRaw = new byte[len];
            Array.Copy(raw, 0, fNameRaw, 0, len);

            string fNameStr = Encoding.UTF8.GetString(fNameRaw);
            string fNameOri = fNameStr;
            int zeroIndx = fNameStr.IndexOf('\0');
            if (zeroIndx != -1)
                fNameStr = fNameStr.Substring(0, zeroIndx);
            fNameStr = root_dir + fNameStr.Replace("/", "\\");

            FileChunking? chunked_file = chunked_files.FirstOrDefault(f => f.fname == fNameStr);

            if (chunked_file != null)
            {
                int readed = chunked_file.readed;
                byte[] fileRaw = chunked_file.raw;

                int remaining = (fileRaw.Length - readed);

                if (remaining + 4 <= raw.Length)
                {
                    byte[] response = new byte[remaining + 4];
                    Array.Copy(BitConverter.GetBytes(remaining), 0, response, 0, 4);
                    Array.Copy(fileRaw, readed, response, 4, remaining);
                    PORT.SendResponse(response);
                    chunked_files.Remove(chunked_file);
                    return;
                }
                else
                {
                    int cap = raw.Length - 4;
                    byte[] response = new byte[cap + 4];
                    Array.Copy(BitConverter.GetBytes(cap), 0, response, 0, 4);
                    Array.Copy(fileRaw, readed, response, 4, cap);
                    PORT.SendResponse(response);

                    chunked_file.readed += cap;
                    return;
                }
            }


            if (File.Exists(fNameStr))
            {
                byte[] fileRaw = File.ReadAllBytes(fNameStr);
                int responseTotalLen = fileRaw.Length;
                int currentResponseLen = (responseTotalLen <= raw.Length ? responseTotalLen : raw.Length - 4);
                byte[] response = new byte[currentResponseLen + 4];

                bool need_chunk = fileRaw.Length > raw.Length;

                if (need_chunk)
                {
                    chunked_files.Add(new FileChunking
                    {
                        fname = fNameStr,
                        readed = currentResponseLen,
                        raw = fileRaw
                    });
                }

                Array.Copy(BitConverter.GetBytes(currentResponseLen), 0, response, 0, 4);
                Array.Copy(fileRaw, 0, response, 4, currentResponseLen);

                PORT.SendResponse(response);
            }
        }

        // =========================
        // NOVAS OPERAÇÕES
        // =========================

        private static void Open(byte[] raw)
        {
            int handle = BitConverter.ToInt32(raw, 0);
            string name = Encoding.UTF8.GetString(raw, 4, raw.Length - 4).TrimEnd('\0');
            string path = root_dir + name.Replace("/", "\\");

            if (open_files.ContainsKey(handle))
                return;

            byte[] info = new byte[4 + 1 + (8 * 4)];
            Array.Copy(BitConverter.GetBytes(info.Length - 4), 0, info, 0, 4);
            info[4] = StorageCmd.F_OPEN;

            if (File.Exists(path))
            {
                FileInfo fi = new FileInfo(path);

                Array.Copy(BitConverter.GetBytes(fi.Length), 0, info, 5, 8);
                Array.Copy(BitConverter.GetBytes(fi.CreationTime.Ticks), 0, info, 13, 8);
                Array.Copy(BitConverter.GetBytes(fi.LastAccessTime.Ticks), 0, info, 21, 8);
                Array.Copy(BitConverter.GetBytes(fi.LastWriteTime.Ticks), 0, info, 29, 8);
            }

            FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            open_files[handle] = new OpenFileDescriptor { Handle = handle, Stream = fs };

            PORT.SendResponse(info);
        }

        private static void Close(byte[] raw)
        {
            int handle = BitConverter.ToInt32(raw, 0);
            if (!open_files.TryGetValue(handle, out var fd)) return;

            fd.Stream.Flush();
            fd.Stream.Dispose();
            open_files.Remove(handle);


            byte[] resp = new byte[4 + 1];
            Array.Copy(BitConverter.GetBytes(resp.Length - 1), 0, resp, 0, 4);
            resp[4] = StorageCmd.F_CLOSE;

            PORT.SendResponse(resp);
        }

        private static void ReadBlock(byte[] raw)
        {
            int handle = BitConverter.ToInt32(raw, 0);
            long pos = BitConverter.ToInt64(raw, 4);
            int len = BitConverter.ToInt32(raw, 12);

            if (!open_files.TryGetValue(handle, out var fd)) return;

            fd.Stream.Position = pos;

            byte[] buffer = new byte[len];
            int read = fd.Stream.Read(buffer, 0, len);

            byte[] resp = new byte[read + 4];
            Array.Copy(BitConverter.GetBytes(read), 0, resp, 0, 4);
            Array.Copy(buffer, 0, resp, 4, read);

            PORT.SendResponse(resp);
        }

        private static void WriteBlock(byte[] raw)
        {
            int handle = BitConverter.ToInt32(raw, 0);
            long pos = BitConverter.ToInt64(raw, 4);
            int len = BitConverter.ToInt32(raw, 12);
            if (!open_files.TryGetValue(handle, out var fd)) return;

            int dataLen = len;
            fd.Stream.Position = pos;
            fd.Stream.Write(raw, 16, dataLen);

            byte[] resp = new byte[4+ 1 + 8];
            Array.Copy(BitConverter.GetBytes(resp.Length - 4), 0, resp, 0, 4);
            resp[4] = StorageCmd.F_WRITE_BLOCK;
            Array.Copy(BitConverter.GetBytes(fd.Stream.Position), 0, resp, 5, 8);

            PORT.SendResponse(resp);
        }

        private static void Length(byte[] raw)
        {
            int handle = BitConverter.ToInt32(raw, 0);
            if (!open_files.TryGetValue(handle, out var fd)) return;

            PORT.SendResponse(BitConverter.GetBytes(fd.Stream.Length));
        }

        private static void Flush(byte[] raw)
        {
            int handle = BitConverter.ToInt32(raw, 0);
            if (!open_files.TryGetValue(handle, out var fd)) return;

            fd.Stream.Flush();

            byte[] resp = new byte[4 + 1];
            Array.Copy(BitConverter.GetBytes(resp.Length - 1), 0, resp, 0, 4);
            resp[4] = StorageCmd.F_FLUSH;

            PORT.SendResponse(resp);
        }

        private static void SetPosition(byte[] raw)
        {
            int handle = BitConverter.ToInt32(raw, 0);
            long pos = BitConverter.ToInt64(raw, 4);

            if (!open_files.TryGetValue(handle, out var fd)) return;

            fd.Stream.Position = pos;


            byte[] resp = new byte[4 + 1];
            Array.Copy(BitConverter.GetBytes(resp.Length - 4), 0, resp, 0, 4);
            resp[4] = StorageCmd.F_SET_POSITION;

            PORT.SendResponse(resp);
        }

        internal static void TURN_OFF()
        {
            if (PORT != null)
                PORT.Disconnect();
        }
    }

    internal class OpenFileDescriptor
    {
        public int Handle;
        public FileStream Stream;
    }
}
