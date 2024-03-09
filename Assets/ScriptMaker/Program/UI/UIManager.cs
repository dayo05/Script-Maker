using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ScriptMaker.Program.UI
{
    public static class UIManager
    {
        private static GameObject canvas;
        private static bool isInitialized;

        private static readonly Stack<UI> uiStack = new();

        public static void Initialize()
        {
            if (isInitialized) throw new Exception("Initialize twice");
            isInitialized = true;
            canvas = GameObject.Find("DialogCanvas");
        }

        public static UI DisplayGui(Type uiType)
        {
            if (!uiType.IsSubclassOf(typeof(UI))) throw new Exception($"Type {uiType} is not instance of UI");
            var uiObject = new GameObject();
            uiObject.transform.SetParent(canvas.transform);
            uiObject.transform.localPosition = Vector3.zero;

            var ui = uiObject.AddComponent(uiType) as UI;
            ui.IsFront = true;
            uiStack.Push(ui);

            EditorMain.ReCalcDisplayUIWithKeepPrevInfo();
            return ui;
        }

        public static UI GetCurrentGui()
        {
            return uiStack.Peek();
        }

        public static bool IsGuiExists()
        {
            return uiStack.Count != 0;
        }

        public static void CloseGui()
        {
            var ui = GetCurrentGui();
            ui.OnClose();
            Object.Destroy(ui.gameObject);
            uiStack.Pop();
            if (uiStack.Count != 0)
                GetCurrentGui().IsFront = true;
            else EditorMain.ReCalcDisplayUIWithKeepPrevInfo();
        }
    }
}