using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mOSLib.functions;
using mOSLib.heap;

namespace mOSLib
{
    public class libmOS
    {
        private env env;
        private IO io;
        private math math;
        private mem mem;
        private mfile file;
        private process process;
        private sys sys;
        private thread thread;
        private time time;
        private console console;
        private libmOS()
        {
            env = new env();
            io = new IO();
            math = new math();
            sys = new sys();
            mem = new mem(sys);
            file = new mfile();
            process = new process(sys);
            console = new console(sys);

            thread = new thread();
            time = new time();
        }

        public static libmOS init()
        {
            return new libmOS();
        }

        public int env_get() => env.get();
        public int env_set() => env.set();

        public int io_open() => io.open();
        public int io_write() => io.write();
        public int io_read() => io.read();
        public int io_seek() => io.seek();
        public int io_flush() => io.flush();

        public int math_abs() => math.abs();
        public int math_rand() => math.rand();
        public int math_srand() => math.srand();
        public int clamp() => math.clamp(); 


        public int mem_alloc(int size) => mem.alloc(size);
        public int mem_free(int addr) => mem.free(addr);
        public int mem_realloc() => mem.realloc();
        public int mem_set(int addr, byte b, int size) => mem.memset(addr, b, size);
        public int mem_cpy(int destAddr, int srcAddr, int count) => mem.memcpy(destAddr, srcAddr, count);
        public int mem_cmp(int addr1, int addr2, int count) => mem.memcmp(addr1, addr2, count);
        public int mem_write(mOSObject o) => mem.write(o);
        public T mem_read<T>(int addr) where T : mOSObject => mem.read<T>(addr);


        public int file_stat() => file.stat();
        public int file_exists() => file.exists();
        public int file_remove() => file.remove();
        public int file_chmod() => file.chmod();


        public int process_spawn() => process.spawn();
        public int process_wait() => process.wait();
        public int process_kill() => process.kill();
        public int process_exit() => process.exit();
        public int process_getpid() => process.getpid();


        public int sys_call(int v0, int? a0 = null, int? a1 = null, int? a2 = null, int? a3 = null, bool k0 = true, int k1 = 0)
            => sys.syscall(v0, a0, a1, a2, a3, k0, k1);
        public int sys_yeld() => sys.yield();
        public int uptime() => sys.uptime();

        public int thread_create() => thread.create();
        public int thread_join() => thread.join();
        public int thread_sleep() => thread.sleep();
        public int thread_current() => thread.current();


        public int time_now() => time.now();
        public int time_sleep() => time.sleep();
        public int time_uptime() => time.uptime();

        public char console_readchar() => console.read_char();
    }
}
