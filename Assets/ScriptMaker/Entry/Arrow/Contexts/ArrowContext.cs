using ScriptMaker.Program.Data;

namespace ScriptMaker.Entry.Arrow.Contexts
{
    public class ArrowContext
    {
        public long From;
        public long To;

        public Option Serialize() => new Option("Context", this.GetType().Name)
            .Append("From", From)
            .Append("To", To);

        public ArrowContext ReadOption(Option opt)
        {
            From = opt["From"].Long();
            To = opt["To"].Long();
            return this;
        }

        public string DisplayName(long NS) => $"{NS}번 화살표 선택됨";
    }
}