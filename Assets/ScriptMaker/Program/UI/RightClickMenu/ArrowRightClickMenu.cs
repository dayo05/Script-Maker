using ScriptMaker.Entry;

namespace ScriptMaker.Program.UI.RightClickMenu
{
    public class ArrowRightClickMenu : RightClickMenu
    {
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            AppendButton("Delete", () =>
            {
                BaseEntry.DeleteContent(NS);
                RightClickMenuHandler.CloseMenu();
            });

            LocateButton();
        }
    }
}