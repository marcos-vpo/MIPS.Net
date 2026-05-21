namespace mOSLib.functions
{
    internal class process
    {
        private readonly sys sys;

        public process(sys sys)
        {
            this.sys = sys;
        }


        public int spawn() { return 0; }
        public int wait() { return 0; }
        public int kill() { return 0; }
        public int pause() => sys.syscall(v0: call_codes.PROCESS_STOP, k0: true);
        public int status() { return 0; }
        public int exit() => sys.syscall(v0: call_codes.PROCESS_EXIT);

        public int getpid()
        {
            var res = sys.syscall(call_codes.SELF_PID, k1: 0);
            return res;
        }
    }
}
