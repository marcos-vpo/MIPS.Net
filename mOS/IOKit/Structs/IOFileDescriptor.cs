using MIPS.Net.InstructionSet;
using mOS.IOKit.Devices;
using mOS.k_objects;
using mOS.kernel;
using mOS.misc;

namespace mOS.IOKit.Structs
{
    internal class IOFileDescriptor : mOSObject
    {
        public override string ToString()
        {
            return $"FD *[{VirtualAddr}] | OPEN: {IsOpen} | LEN: {Length} | POS: {Position} | {Path}";
        }

        // Identificador único do FD
        [FieldOrder(0)]
        public int Handle { get; set; }

        // Posição lógica atual no arquivo
        [FieldOrder(2)]
        public long Position { get; set; }

        // Tamanho atual conhecido do arquivo
        [FieldOrder(3)]
        public long Length { get; set; }

        [FieldOrder(4)]
        public long CreationTime { get; set; }

        [FieldOrder(5)]
        public long LastAccessTime { get; set; }

        [FieldOrder(6)]
        public long LastWriteTime { get; set; }

        // Indica se o descritor ainda está válido
        [FieldOrder(4)]
        public bool IsOpen { get; set; }

        // Caminho lógico usado na abertura ("/etc/video.mp4")
        [FieldOrder(5)]
        public string Path { get; set; }

        internal static IOFileDescriptor Open(string path, IOStorageService storage)
        {
            if (!storage.IOServiceActive) return null;

            IOFileDescriptor fd = new IOFileDescriptor
            {
                Path = path
            };

            var heap = mos_kernel.KernelHeap;

            heap.WriteObject(fd);
            fd.Handle = fd.VirtualAddr;

            IOResult? res = storage.Open(handle: fd.VirtualAddr, path);
            if (res == null) return null;

            if (res.Data[0] == IOStorageCommand.F_OPEN)
            {
                byte[] data = new byte[(8 * 4)];
                Array.Copy(res.Data, 1, data, 0, data.Length);
                long length = BitConverter.ToInt64(data[0..8]);
                long creationTime = BitConverter.ToInt64(data[8..16]);
                long lastAcessTime = BitConverter.ToInt64(data[16..24]);
                long lastWriteTime = BitConverter.ToInt64(data[24..32]);

                fd.Length = length;
                fd.CreationTime = creationTime;
                fd.LastAccessTime = lastAcessTime;
                fd.LastWriteTime = lastWriteTime;
                fd.IsOpen = true;

                heap.WriteObject(fd);
            }

            return fd;
        }

        internal IOResult? ReadAt(long pos, int len, IOStorageService stg)
        {
            var res = stg.Read(Handle, pos, len);
            if (res != null)
            {
                LastAccessTime = DateTime.Now.Ticks;
                Position += len;
                mos_kernel.KernelHeap.WriteObject(this);
            }
            return res;
        }

        internal IOResult? Read(long len, IOStorageService stg)
        {
            var res = stg.Read(Handle, Position, (int)len);
            if (res != null)
            {
                LastAccessTime = DateTime.Now.Ticks;
                Position += len;
                mos_kernel.KernelHeap.WriteObject(this);
            }

            return res;
        }

        internal bool Write(byte[] block, IOStorageService stg)
        {
            Length = stg.Write(Handle, Position, block);

            if (Length > 0)
            {
                Position += block.Length;
                LastWriteTime = DateTime.Now.Ticks;
                mos_kernel.KernelHeap.WriteObject(this);
                return true;
            }
            return false;
        }

        internal bool WriteAt(int pos, byte[] block, IOStorageService stg)
        {
            Length = stg.Write(Handle, pos, block);
            if (Length > 0)
            {
                LastWriteTime = DateTime.Now.Ticks;
                mos_kernel.KernelHeap.WriteObject(this);
                return true;
            }
            return false;
        }

        internal long GetLength(IOStorageService stg)
        {
            Length = stg.Length(Handle);
            mos_kernel.KernelHeap.WriteObject(this);
            return Length;
        }

        internal bool Flush(IOStorageService stg)
        {
            bool flush = stg.Flush(Handle);

            return flush;
        }

        internal bool Close(IOStorageService stg)
        {
            var closed = stg.Close(Handle);

            if (closed)
            {
                var heap = mos_kernel.KernelHeap;
                heap.FreeObject(Handle);

                IsOpen = false;
                LastAccessTime = 0;
                LastWriteTime = 0;
                Path = "";
                CreationTime = 0;
                Length = 0;

            }

            return closed;
        }

        internal bool SetPosition(long pos, IOStorageService stg)
        {
            if ((pos - 1) > Length) return false;
            bool set = stg.SetPosition(Handle, pos);
            if (set)
            {
                Position = pos;
                mos_kernel.KernelHeap.WriteObject(this);
            }
            return set;
        }
    }
}
