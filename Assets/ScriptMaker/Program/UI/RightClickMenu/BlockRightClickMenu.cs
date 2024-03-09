using ScriptMaker.Entry;
using ScriptMaker.Entry.Block;
using ScriptMaker.Util;

namespace ScriptMaker.Program.UI.RightClickMenu
{
    public class BlockRightClickMenu : RightClickMenu
    {
        protected override void Start()
        {
            base.Start();

            AppendButton("Copy", () =>
            {
                BlockHandler.CreateEntry(BlockHandler.GetBlockContent(NS).Clone(),
                    new Point(BlockHandler.Blocks[NS].Point.X + 10, BlockHandler.Blocks[NS].Point.Y - 10));
                RightClickMenuHandler.CloseMenu();
            });

            AppendButton("Edit", () =>
            {
                if (EditorMain.IsIgnoreSelectionMod) return;
                EditEntryGui.OpenEditDialog(NS);
                RightClickMenuHandler.CloseMenu();
            });

            AppendButton("Delete", () =>
            {
                if (EditorMain.IsIgnoreSelectionMod) return;
                BaseEntry.DeleteContent(NS);
                RightClickMenuHandler.CloseMenu();
            });

            LocateButton();
        }
    }
}