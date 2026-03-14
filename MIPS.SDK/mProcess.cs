namespace mOSLib
{
    public static class mProcess
    {
        public static int spawn() { return 0; }
        public static int wait() { return 0; }
        public static int kill() { return 0; }
        public static int status() { return 0; }
        public static int exit() { return 0; }
        public static int getpid()
        {
            var res = mSys.syscall(2, k1: 0);
            return res;
        }
    }
}
