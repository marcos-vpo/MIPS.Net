namespace mOSLib.heap
{
    public abstract class mOSObject
    {
        private object o;
        public void Set(object o)
        {
            this.o = o;

            if (_serializer == null)
            {
                _serializer = new mOSObjectSerializer();
                VirtualAddr = -1;
            }
        }

        public int VirtualAddr { get; private set; }
        public short Flags { get; private set; }
        public short mOSType { get; }

        private mOSObjectSerializer _serializer;

        public byte[] Serialize() => _serializer.Serialize(o);

        public void Desserialize(byte[] data) => _serializer.Desserialize(o, data);

        internal void SetVirtualAddr(int virtualAddr)
        {
            VirtualAddr = virtualAddr;
        }

        internal void SetFlags(short flags)
        {
            Flags = flags;
        }
    }
}
