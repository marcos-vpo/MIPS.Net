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
        public int status() { return 0; }
        public int exit() { return 0; }

        public int getpid()
        {
            var res = sys.syscall(2, k1: 0);
            return res;
        }
    }
}
