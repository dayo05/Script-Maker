using System.Linq;
using ScriptMaker.Program.Data;
using UnityEngine;

namespace ScriptMaker.Entry.Block
{
    public class BeginBlock : BaseBlock
    {
        protected override string Text => $"Begin: {Context["Label"].String()}";
        protected override Color Color => Color.green;

        protected override bool ValidateBlock(long NS, Option c)
        {
            return base.ValidateBlock(NS, c) && BlockHandler.Blocks.Count(x =>
                x.Value is BeginBlock && x.Value.Context["Label"].String() == c["Label"].String()) == 0;
        }
    }
}