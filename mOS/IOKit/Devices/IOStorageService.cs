using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Abi;
using mOS.IOKit.Structs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace mOS.IOKit.Devices
{
    internal class IOStorageService : ABIManaged, IDisposable
    {
        private readonly IOService io_service;
        public bool IOServiceActive { get; private set; }
        public IOStorageService()
        {
            io_service = IORegistry.GetService(IODeviceClass.STORAGE)
               ?? throw new InvalidOperationException("Display IOService not found");
            IOServiceActive = io_service.Open(IODeviceOperation.READ, IOStorageCommand.IDLE);
            if (!IOServiceActive) io_service = null;
        }

        public Directory_Info[] ListDirectories(string baseDir)
        {
            if (!IOServiceActive) return [];

            List<Directory_Info> dis = new List<Directory_Info>();

            io_service.SetMode(IOStorageCommand.FETCH_DIR_LIST);
            io_service.PrepareBuffer(Encoding.ASCII.GetBytes(baseDir));
            io_service.Fire();

            IOResult? result = io_service.Result();

            if (result == null) return [];


            byte[] res_b = result.Data;
       //     m_read(io_service.DeviceBufferAddr() + 4, ref res_b);

            io_service.SetMode(IOStorageCommand.IDLE);
            //    io_service.Close();

            // dir tuple: [ creation (8b) + last_change (8b) + files (4b) + size (8b) + name (Nb....\0) ]

            int readed = 0;

            byte[] creation_b = new byte[8];
            byte[] last_write_b = new byte[8];
            byte[] last_access_b = new byte[8];
            byte[] files_b = new byte[4];
            byte[] size_b = new byte[8];

            StringBuilder sb = new StringBuilder();

            while (readed < res_b.Length)
            {
                Array.Copy(res_b, readed, creation_b, 0, 8);
                readed += 8;

                Array.Copy(res_b, readed, last_write_b, 0, 8);
                readed += 8;

                Array.Copy(res_b, readed, last_access_b, 0, 8);
                readed += 8;

                Array.Copy(res_b, readed, files_b, 0, 4);
                readed += 4;

                Array.Copy(res_b, readed, size_b, 0, 8);
                readed += 8;

                long creation = BitConverter.ToInt64(creation_b);  // Começa no índice 0
                long lastWrite = BitConverter.ToInt64(last_write_b);  // Pula 8 bytes
                long lastAccess = BitConverter.ToInt64(last_access_b); // Pula mais 8
                int files = BitConverter.ToInt32(files_b); // Pula mais 4
                long size = BitConverter.ToInt64(size_b); // Pula mais 8

                byte[] char_ab = [1];
                int nameStart = readed;
                while (readed < res_b.Length)
                {
                    byte b = res_b[readed++];
                    if (b == 0) break;
                    sb.Append((char)b);
                }

                Directory_Info di = new Directory_Info
                {
                    Creation = BitConverter.ToInt64(creation_b),
                    Last_Write = BitConverter.ToInt64(last_write_b),
                    Last_Access = BitConverter.ToInt64(last_access_b),
                    Files = BitConverter.ToInt32(files_b),
                    Size = BitConverter.ToInt64(size_b),
                    Name = sb.ToString()
                };

                sb.Clear();
                dis.Add(di);
            }

            return dis.ToArray();
        }

        public void Dispose()
        {
            if (!IOServiceActive) return;
            if (!io_service.IsClosed())
            {
                io_service.Close();
            }
        }

        internal IOResult? ReadAllBytes(string path)
        {
            if (!IOServiceActive) return null;

            io_service.SetMode(IOStorageCommand.F_READ_ALL);
            io_service.PrepareBuffer(Encoding.UTF8.GetBytes(path));
            io_service.Fire();

            var res = io_service.Result();

            io_service.SetMode(IOStorageCommand.IDLE);

            return res;
        }

        internal IOResult? Open(int handle, string fileName)
        {
            if (!IOServiceActive) return null;

            io_service.SetMode(IOStorageCommand.F_OPEN);
            byte[] cmd = new byte[4 + fileName.Length + 1];
            byte[] nameBytes = Encoding.UTF8.GetBytes(fileName);

            Array.Copy(BitConverter.GetBytes(handle), 0, cmd, 0, 4);
            Array.Copy(nameBytes, 0, cmd, 4, nameBytes.Length);

            io_service.PrepareBuffer(cmd);
            io_service.Fire();

            var res = io_service.Result();

            io_service.SetMode(IOStorageCommand.IDLE);

            return res;
        }

        internal bool Close(int handle)
        {
            if (!IOServiceActive) return false;

            io_service.SetMode(IOStorageCommand.F_CLOSE);
            io_service.PrepareBuffer(BitConverter.GetBytes(handle));
            io_service.Fire();

            var res = io_service.Result();
            if (res == null) return false;

            io_service.SetMode(IOStorageCommand.IDLE);

            return res.Data[0] == IOStorageCommand.F_CLOSE;
        }

        internal IOResult? Read(int handle, long position, int length)
        {
            if (!IOServiceActive) return null;
            io_service.SetMode(IOStorageCommand.F_READ_BLOCK);

            byte[] args = new byte[16];
            Array.Copy(BitConverter.GetBytes(handle), 0, args, 0, 4);
            Array.Copy(BitConverter.GetBytes(position), 0, args, 4, 8);
            Array.Copy(BitConverter.GetBytes(length), 0, args, 12, 4);

            io_service.PrepareBuffer(args);
            io_service.Fire();

            var res = io_service.Result();
            if (res == null) return null;

            io_service.SetMode(IOStorageCommand.IDLE);

            return res;
        }

        internal long Write(int handle, long position, byte[] block)
        {
            if (!IOServiceActive) return 0;
            int physicalBufferLimit = io_service.BufferLength();
            // precisa tratar a questao do limite do physicalBufferLimit
            // se for o caso, enviar chunkeado

            io_service.SetMode(IOStorageCommand.F_WRITE_BLOCK);

            byte[] args = new byte[4 + 8 + +4 + block.Length];

            Array.Copy(BitConverter.GetBytes(handle), 0, args, 0, 4);
            Array.Copy(BitConverter.GetBytes(position), 0, args, 4, 8);
            Array.Copy(BitConverter.GetBytes(block.Length), 0, args, 12, 4);
            Array.Copy(block, 0, args, 16, block.Length);

            io_service.PrepareBuffer(args);
            io_service.Fire();

            var res = io_service.Result();
            if (res == null) return 0;

            io_service.SetMode(IOStorageCommand.IDLE);

            if (res.Data[0] == IOStorageCommand.F_WRITE_BLOCK)
            {
                long newLength = BitConverter.ToInt64(res.Data[1..9]);
                return newLength;
            }

            return 0;
        }

        internal long Length(int handle)
        {
            if (!IOServiceActive) return 0;

            io_service.SetMode(IOStorageCommand.F_LENGTH);

            io_service.PrepareBuffer(BitConverter.GetBytes(handle));
            io_service.Fire();

            var res = io_service.Result();
            if (res == null) return -1;

            io_service.SetMode(IOStorageCommand.IDLE);

            try
            {
                long len = BitConverter.ToInt64(res.Data[0..8]);
                return len;
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        internal bool Flush(int handle)
        {
            if (!IOServiceActive) return false;

            io_service.SetMode(IOStorageCommand.F_FLUSH);

            io_service.PrepareBuffer(BitConverter.GetBytes(handle));
            io_service.Fire();

            var res = io_service.Result();
            if (res == null) return false;

            io_service.SetMode(IOStorageCommand.IDLE);

            return res.Data[0] == IOStorageCommand.F_FLUSH;
        }

        internal bool SetPosition(int handle, long position)
        {
            if (!IOServiceActive) return false;

            io_service.SetMode(IOStorageCommand.F_SET_POSITION);


            byte[] cmd = new byte[12];
            Array.Copy(BitConverter.GetBytes(handle), 0, cmd, 0, 4);
            Array.Copy(BitConverter.GetBytes(position), 0, cmd, 4, 8);

            io_service.PrepareBuffer(cmd);
            io_service.Fire();

            var res = io_service.Result();
            if (res == null) return false;

            io_service.SetMode(IOStorageCommand.IDLE);

            return res.Data[0] == IOStorageCommand.F_SET_POSITION;
        }
    }
}
