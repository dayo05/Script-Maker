using ScriptMaker.Entry.Block.Contexts.Dialog;
using ScriptMaker.Program.Data;

namespace ScriptMaker.Entry.Block.Contexts
{
    public class BeginBlockContextEditDialog : ContextEditDialog
    {
        protected override void Initialize()
        {
            Context.SetDefaultOf("Label", "default");
            AddSingleLineInputField("Data", str => Context.Set("Label", str), Context["Label"].String());
        }
    }
}