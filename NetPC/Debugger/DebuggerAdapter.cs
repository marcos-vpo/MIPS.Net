using System.IO;
using MIPS.Net.Debugger;
using MIPS.Net.SoC;
using MIPS.Net.SoC.__program;

namespace NetPC.Debugger
{
    public class DebuggerAdapter : DebuggerBridge
    {
        private Func<int, DebuggerView> _onRequestView;
        private DebuggerView _view;
        public DebuggerAdapter(Func<int, DebuggerView> onRequestView)
        {
            this._onRequestView = onRequestView;
            Disable();
        }
        public override void OnRequest()
        {
            if (_view == null)
            {
                _view = _onRequestView(0);
                Step(1);
            }
        }

        public override void StateUpdate(ProgramContext pg, int instruction, Registers registers, Memory memory)
        {
            if (State == DebuggerState.DISCONNECTED) return;

            _view.UpdateRegisters(registers);

            int index = 0;

            int pc = registers[$"pc"];
            int rotule = pg.GetCurrentRotule(pc);
            TracingRotule? rtle = pg.GetRotule(rotule);
            if (rtle == null)
            {
                Disable();
                _view.Invoke(() => _view.Close());
                return;
            }
            FileInfo fi = new FileInfo(pg.Tracing.SourceFile);
            _view.SetRotule($"[{rtle.Name}]: {fi.Name}, ln {rtle.Ln} -- ADDR *[0x{rtle.StartAddr:X2}] ~ *[0x{rtle.EndAddr:X2}]");


            if (!current_rtl_instructions.Any(i => i.Address == pc))
            {

                if (rotule > -1)
                {

                    if (rtle == null) return;


                    current_rtl_instructions = pg.GetRotuleInstructions(rotule);
                    _view.FillInstructions(current_rtl_instructions);
                }
            }

            index = current_rtl_instructions.FindIndex(r => r.Address == registers[$"pc"]);


            _view.SelectInstruction(index);
        }

        public override void MemAccess(int addr)
        {
            if (State == DebuggerState.DISCONNECTED) return;
            _view?.MemAccess(addr);
        }

        public override void IOAccess(int addr)
        {
            if (State == DebuggerState.DISCONNECTED) return;
            _view?.IOAccess(addr);
        }

        List<RotuleInstructionVM> current_rtl_instructions = new List<RotuleInstructionVM>();
        public override void ProgramSwitching(ProgramContext pg, int pc)
        {
            if (State == DebuggerState.DISCONNECTED) return;
            if (pg.Tracing == null)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Debugger Tracing File|*.mdbx";
                ofd.Title = "Attatch Tracing File";
                _view.Invoke(() => ofd.ShowDialog());

                if (File.Exists(ofd.FileName))
                {
                    try
                    {
                        string json = File.ReadAllText(ofd.FileName);
                        DebuggerTracingObject dto = System.Text.Json.JsonSerializer.Deserialize<DebuggerTracingObject>(json); //JsonConvert.DeserializeObject<DebuggerTracingObject>(json);
                        pg.AttatchTracing(dto);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Invalid tracing file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            int rotule = pg.GetCurrentRotule(pc);
            //          if(rotule == -1) rotule =  pg.GetCurrentRotule(pc + 4);
            if (rotule > -1)
            {
                TracingRotule? rtle = pg.GetRotule(rotule);
                if (rtle == null) return;

                FileInfo fi = new FileInfo(pg.Tracing.SourceFile);

                _view.SetRotule($"[{rtle.Name}]: {fi.Name}, ln {rtle.Ln} -- ADDR *[0x{rtle.StartAddr:X2}] ~ *[0x{rtle.EndAddr:X2}]");

                current_rtl_instructions = pg.GetRotuleInstructions(rotule);
                _view.FillInstructions(current_rtl_instructions);
            }
        }
    }
}
