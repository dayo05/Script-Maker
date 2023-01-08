using System;
using System.Collections.Generic;
using ScriptMaker.Entry.Block.Contexts.Dialog;
using UnityEngine.UI;

namespace ScriptMaker.Program.Mod
{
    public static class ModEvent
    {
        public static class Context
        {
            public static Action<ContextEditDialog> OpenDialogEvent = _ => { };
            public static Action<ContextEditDialog, InputField> CreateInputFieldEvent = (_, _) => { };
            public static Action<ContextEditDialog, Button> CreateButtonEvent = (_, _) => { };
            public static Action<ContextEditDialog, Text> CreateTextEvent = (_, _) => { };
            public static Action<ContextEditDialog, Dropdown, List<(string displayName, string serializedName)>> CreateDropdownEvent = (_, _, _) => { };
            public static Action<ContextEditDialog, Dropdown, string> OnDropdownMenuChangedEvent = (_, _, _) => { };
            public static Action<ContextEditDialog, Image> CreateImageEvent = (_, _) => { };
            public static Action<ContextEditDialog, Toggle> CreateCheckboxEvent = (_, _) => { };
            public static Action PostLoadingEvent = () => { };
        }
    }
}