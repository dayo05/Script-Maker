using System;
using System.Collections.Generic;
using System.Linq;
using ScriptMaker.Program.Data;
using ScriptMaker.Program.Mod;
using ScriptMaker.Program.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ScriptMaker.Entry.Block.Contexts.Dialog
{
    public abstract class ContextEditDialog : UI
    {
        public EditEntryGui baseDialog;

        protected readonly InputField.OnValidateInput floatValidator = (text, index, addedChar) =>
            addedChar is not '.' and < '0' or > '9'
                ? '\0'
                : addedChar == '.' && text.Contains('.')
                    ? '\0'
                    : addedChar;

        protected VStack PrimaryVStack { get; private set; }
        private UIStack stack;
        public Option Context { get; set; }
        public override bool IsFront => baseDialog.IsFront;

        public void Start()
        {
            stack = PrimaryVStack = VStack.Create();
            stack.transform.SetParent(transform);
            stack.SelfTransform.anchoredPosition = new Vector3(65, -30);

            Initialize();
            ModEvent.Context.OpenDialogEvent(this);
        }

        private void Update()
        {
            if(stack.needsRecalculate)
                stack.UpdateBatch();
        }

        protected HStack PushH(Action act, string name = "")
        {
            var hStack = PushH(name);
            act();
            Pop();
            return hStack;
        }

        protected VStack PushV(Action act, string name = "")
        {
            var vStack = PushV(name);
            act();
            Pop();
            return vStack;
        }

        protected HStack PushH(string name = "")
        {
            stack = HStack.Create(stack, name);
            return (HStack) stack;
        }

        protected VStack PushV(string name = "")
        {
            stack = VStack.Create(stack, name);
            return (VStack) stack;
        }

        protected void Pop()
        {
            stack = stack.parent;
        }

        protected virtual void Initialize()
        {
        }

        public void AddInputField(string name, UnityAction<string> onValueChanged, string text = "", float width = 1050,
            float height = 365,
            float xBias = 0, int fontSize = 18, UnityAction<InputField> transform = null)
        {
            var ik = InnerStack.Create(stack, name);
            ik.spaceLeft = xBias;

            var rect = ik.SelfTransform;
            rect.anchorMax = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.sizeDelta = new Vector2(width, height);

            var g = ik.gameObject;

            var ipf = g.AddComponent<InputField>();
            ipf.image = g.AddComponent<Image>();
            ipf.lineType = InputField.LineType.MultiLineNewline;

            CreateText(out var gif, name + "InputText", "", g);
            gif.GetComponent<RectTransform>().sizeDelta = new Vector2(width - 20, height - 15);
            gif.GetComponent<RectTransform>().localPosition = new Vector3(width / 2, -height / 2);

            ipf.textComponent = gif.GetComponent<Text>();
            ipf.text = text;
            ipf.textComponent.fontSize = fontSize;
            ipf.onValueChanged.AddListener(onValueChanged);

            transform?.Invoke(ipf);
            ModEvent.Context.CreateInputFieldEvent(this, ipf);
        }

        public void AddButton(string text, UnityAction onClick, float width = 200, float height = 50, float xBias = 0,
            int fontSize = 18, UnityAction<Button> transform = null)
        {
            var btn = CreateButton(out var g, text + "Button", onClick);
            var ik = InnerStack.Create(g, stack);
            var rect = ik.SelfTransform;
            rect.anchorMax = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.sizeDelta = new Vector2(width, height);

            CreateText(out var gt, "Text", text, g, Color.black, TextAnchor.MiddleCenter);
            gt.GetComponent<RectTransform>().localPosition = new Vector3(width / 2, -height / 2);
            gt.GetComponent<Text>().fontSize = fontSize;

            transform?.Invoke(btn);
            ModEvent.Context.CreateButtonEvent(this, btn);
        }

        public void AddSingleLineInputField(string name,
            UnityAction<string> onValueChanged, string text = "", float width = 1050, float height = 30,
            float xBias = 0, int fontSize = 18, Action<InputField> transform = null)
        {
            var ik = InnerStack.Create(stack, name);
            ik.spaceLeft = xBias;
            var rect = ik.SelfTransform;
            rect.anchorMax = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.sizeDelta = new Vector2(width, height);

            var g = ik.gameObject;
            var ipf = g.AddComponent<InputField>();
            ipf.image = g.AddComponent<Image>();
            ipf.lineType = InputField.LineType.SingleLine;

            CreateText(out var gif, name + "InputText", "", g);
            gif.GetComponent<RectTransform>().sizeDelta = new Vector2(width - 20, height - 5);
            gif.GetComponent<RectTransform>().localPosition = new Vector3(width / 2, -height / 2);
            ipf.textComponent = gif.GetComponent<Text>();
            ipf.text = text;
            ipf.textComponent.fontSize = fontSize;
            ipf.onValueChanged.AddListener(onValueChanged);

            transform?.Invoke(ipf);
            ModEvent.Context.CreateInputFieldEvent(this, ipf);
        }

        public void AddText(string name, string text, float width = 150, float height = 25, float xBias = 0,
            int fontSize = 18, Action<Text> transform = null, Color? color = null)
        {
            color ??= Color.black;
            var t = CreateText(out var g, name, text, color, TextAnchor.MiddleCenter);
            t.fontSize = fontSize;
            var ik = InnerStack.Create(g, stack);

            var rect = ik.SelfTransform;
            rect.anchorMax = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.sizeDelta = new Vector2(width, height);
            ik.spaceLeft = xBias;

            transform?.Invoke(t);
            ModEvent.Context.CreateTextEvent(this, t);
        }

        public void AddDropdownMenu(string name, List<(string displayName, string serializedName)> menus,
            Action<string> onValueChanged,
            string defaultValue = null, float width = 300, float height = 60, float xBias = 0, int fontSize = 24,
            Action<Dropdown> transform = null)
        {
            var g = (Instantiate(Resources.Load("Dropdown"), this.transform, true) as GameObject)!;
            var ik = InnerStack.Create(g, stack);
            g.name = name;
            var rect = ik.SelfTransform;
            rect.anchorMax = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.sizeDelta = new Vector2(width, height);
            ik.spaceLeft = xBias;

            g.transform.Find("Label").GetComponentInChildren<Text>().fontSize = fontSize;

            {
                var template = g.transform.Find("Template").Find("Viewport").Find("Content");
                template.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 56);
                var item = template.Find("Item");
                item.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 40);
                var label = item.Find("Item Label");
                label.GetComponent<Text>().fontSize = 24;
            }

            var dropdown = g.GetComponent<Dropdown>();

            transform?.Invoke(dropdown);
            ModEvent.Context.CreateDropdownEvent(this, dropdown, menus);

            dropdown.AddOptions(menus.Select(x => x.displayName).ToList());
            dropdown.RefreshShownValue();
            dropdown.value = dropdown.options.FindIndex(x =>
                x.text == menus.First(x => x.serializedName == defaultValue).displayName);
            dropdown.onValueChanged.AddListener(x =>
            {
                var k = menus.First(x2 => x2.displayName == dropdown.options[x].text)
                    .serializedName;
                onValueChanged?.Invoke(k);
                ModEvent.Context.OnDropdownMenuChangedEvent(this, dropdown, k);
            });
        }

        public void AddImage(string name, float width, float height, float xBias = 0, Action<Image> transform = null)
        {
            var x = CreateImage(out var g, name);
            var ik = InnerStack.Create(g, stack);
            var rect = ik.SelfTransform;
            rect.anchorMax = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.sizeDelta = new Vector2(width, height);
            ik.spaceLeft = xBias;

            transform?.Invoke(x);
            ModEvent.Context.CreateImageEvent(this, x);
        }

        public void AddCheckbox(string name, string text, UnityAction<bool> onValueChanged, bool defaultValue = false,
            float width = 320, float height = 40, float xBias = 0, Action<Toggle> transform = null)
        {
            var x = CreateCheckbox(out var g, name, text, defaultValue);
            var ik = InnerStack.Create(g, stack);
            var rect = ik.SelfTransform;
            rect.anchorMax = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.sizeDelta = new Vector2(width, height);
            ik.spaceLeft = xBias;

            x.onValueChanged.AddListener(onValueChanged);

            transform?.Invoke(x);
            ModEvent.Context.CreateCheckboxEvent(this, x);
        }
    }
}