using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ScriptMaker.Program.UI.RightClickMenu
{
    public static class RightClickMenuHandler
    {
        public static long latestNS = -1;
        private static bool isInitialized;
        private static GameObject canvas;
        public static RightClickMenu currentMenu { get; private set; }

        public static void Initialize()
        {
            if (isInitialized) throw new Exception("Initialize twice");
            isInitialized = true;
            canvas = GameObject.Find("DialogCanvas");
        }

        public static RightClickMenu OpenMenu(Type menuType, long NS)
        {
            if (!menuType.IsSubclassOf(typeof(RightClickMenu)))
                throw new Exception($"Type {menuType} is not instance of UI");
            if (currentMenu is not null)
            {
                currentMenu.OnClose();
                CloseMenu();
            }

            var rightClickMenuObj = new GameObject();
            rightClickMenuObj.transform.SetParent(canvas.transform);
            var menu = rightClickMenuObj.AddComponent(menuType) as RightClickMenu;

            menu.NS = NS;
            currentMenu = menu;
            latestNS = NS;
            return currentMenu;
        }

        public static void CloseMenu()
        {
            if (currentMenu is null) return;
            Object.Destroy(currentMenu.gameObject);
            currentMenu = null;
        }

        public static bool IsPointerHoverMenu()
        {
            return currentMenu is not null && currentMenu.IsPointerOnMenu;
        }
    }
}