using ScriptMaker.Program.Data;

namespace ScriptMaker.Entry.Arrow.Contexts
{
    public class ArrowContext: BaseArrowContext
    {
        public string Key = "0default";

        public override Option Serialize() => base.Serialize()
            .Append("Key", Key);

        public override BaseArrowContext ReadOption(Option opt)
        {
            base.ReadOption(opt);
            Key = opt["Key"].String();
            return this;
        }
    }
}