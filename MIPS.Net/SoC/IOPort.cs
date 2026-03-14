namespace MIPS.Net.SoC
{
    using System;
    using System.Collections.Generic;

    // Interface para dispositivos
    public interface IOPort
    {
        public object Tag { get; set; }

        public byte ID { get; set; }
        public byte Class { get; set; }

        public int DataBufferLen { get; set; }
        public int DevicePort { get; set; }
        public int INTR_DATA_RECEIVED { get; set; }
        public int INTR_DEV_CONNECT { get; set; }
        public int INTR_DEV_DISCONNECT { get; set; }
        public int INTR_SEND_FAIL { get; set; }
        public int INTR_REC_FAIL { get; set; }

        public void BusConnected(int addr, IOBUS bus, InterruptionEntry[] port_interruptions);
        public int GetMemAddress();

        public void TurnOn();
        public void TurnOff();

        public void ReadRequest(byte[] deviceMemory, Func<KeyValuePair<bool, byte[]>, int> callBack = null);
        public void WriteRequest(byte[] data, byte[] deviceMemory, Func<KeyValuePair<bool, byte[]>, int> callBack = null);
        void ForceHalt();
    }
}
