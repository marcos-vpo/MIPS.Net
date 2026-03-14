using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPS.Net.SoC;
using MIPS.Net.SoC.__program;

namespace MIPS.Net.Debugger
{
    public enum DebuggerState
    {
        DISCONNECTED = 0,
        PAUSED = 1,
        STEP = 2,
        CONTINUE = 3
    }
    public abstract class DebuggerBridge
    {
        public DebuggerState State { get; private set; }

        protected DebuggerBridge()
        {
            State = DebuggerState.DISCONNECTED;
        }


        public abstract void StateUpdate(ProgramContext program, int instruction, Registers registers, Memory memory);

        public abstract void MemAccess(int addr);
        public abstract void IOAccess(int addr);
        public abstract void OnRequest();


        public void RequestDebugger()
        {
            Enable();
            OnRequest();
            ProgramSwitching(MIPS_CPU.CurrentProgram, MIPS_CPU.Instance.Registers["$pc"]);
            StateUpdate(MIPS_CPU.CurrentProgram, MIPS_CPU.Instance.Registers["$pc"], MIPS_CPU.Instance.Registers, MotherBoard.Instance.Memory);
        }

        public void Enable()
        {
            State = DebuggerState.PAUSED;
        }

        public void Disable()
        {
            State = DebuggerState.DISCONNECTED;
            step_count = 0;
            current_step = 0;
        }


        private int step_count = 0;
        private int current_step = 0;
        public void Step(int count = 1)
        {
            State = DebuggerState.STEP;
            step_count = count;
            current_step = 1;
        }

        public void DoStep()
        {
            if (State == DebuggerState.DISCONNECTED) return;
            if (current_step == step_count)
            {
                State = DebuggerState.PAUSED;
                current_step = 0;
                step_count = 0;
                return;
            }
            current_step += 1;

            var cpu = MIPS_CPU.Instance;
       
        }

        internal void Continue()
        {
            if(State == DebuggerState.DISCONNECTED) return;
            State = DebuggerState.CONTINUE;
        }

        public abstract void ProgramSwitching(ProgramContext pg, int pc);
    }
}
