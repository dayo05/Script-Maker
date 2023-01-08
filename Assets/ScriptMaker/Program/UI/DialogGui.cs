using System;
using ScriptMaker.Util;
using UnityEngine;
using UnityEngine.UI;

namespace ScriptMaker.Program.UI
{
    public enum DialogType
    {
        Ok,
        YesNo,
        YesNoCancel
    }

    public enum DialogResult
    {
        Yes,
        No, 
        Cancel
    }
    public class DialogGui: UI
    {
        public string message;
        public DialogType type;
        private Action<DialogResult> callback;

        private void Start()
        {
            CreateObject(out var background, "Background");
            background.AssignRectTransform();
            background.SetObjectSize(1280, 720);
            background.SetObjectDefaultPos();
            var bgi = background.AddComponent<Image>();
            bgi.color = Color.gray;

            CreateText(out var textObj, "Message", message);
            textObj.SetObjectSize(1200, 720);
            textObj.SetObjectPos(10, -200);

            CreateButton(out var okButtonObj, "OkButton", () =>
            {
                if (!IsFront) return;
                UIManager.CloseGui();
                callback(DialogResult.Yes);
            });
            okButtonObj.SetObjectPos(450, -300);
            okButtonObj.SetObjectSize(180, 50);

            CreateButton(out var noButtonObj, "NoButton", () =>
            {
                if (!IsFront) return;
                UIManager.CloseGui();
                callback(DialogResult.No);
            });
            noButtonObj.SetObjectPos(225, -300);
            noButtonObj.SetObjectSize(180, 50);

            CreateButton(out var cancelButtonObj, "CancelButton", () =>
            {
                if (!IsFront) return;
                UIManager.CloseGui();
                callback(DialogResult.Cancel);
            });
            cancelButtonObj.SetObjectPos(0, -300);
            cancelButtonObj.SetObjectSize(180, 50);

            switch (type)
            {
                case DialogType.Ok:

                    noButtonObj.SetActive(false);
                    cancelButtonObj.SetActive(false);
                    break;
                case DialogType.YesNo:
                    cancelButtonObj.SetActive(false);
                    break;
                case DialogType.YesNoCancel:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CreateText(out var okButtonTextObj, "OkButtonText", "OK", okButtonObj, Color.black,
                TextAnchor.MiddleCenter);
            okButtonTextObj.SetObjectDefaultPos();
            CreateText(out var noButtonTextObj, "NoButtonText", "No", noButtonObj, Color.black,
                TextAnchor.MiddleCenter);
            noButtonTextObj.SetObjectDefaultPos();
            CreateText(out var cancelButtonTextObj, "CancelButtonText", "Cancel", cancelButtonObj, Color.black,
                TextAnchor.MiddleCenter);
            cancelButtonTextObj.SetObjectDefaultPos();
        }

        public static void DisplayDialog(string message, DialogType type = DialogType.Ok, Action<DialogResult> callback = null)
        {
            Log.Info("Try to display dialog: " + message);
            var gui = (DialogGui) UIManager.DisplayGui(typeof(DialogGui));
            gui.message = message;
            gui.type = type;
            gui.callback = callback;
        }
    }
}