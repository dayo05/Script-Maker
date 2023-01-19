using System;
using ScriptMaker.Program.Data;

namespace ScriptMaker.Program.UI
{
    public class ScriptSettingsGui: UI
    {
        void Start()
        {
            AddDefaultUI();

            CreateCheckbox(out var canMovePrevCheckBox, "MovePreviousCheckbox", "왼쪽화살표로 뒤로가기 허용").onValueChanged.AddListener(
                v =>
                {
                    ScriptSettings.CanMovePrevious = v;
                });
            canMovePrevCheckBox.SetObjectPos(-350, 175);
        }
    }
}