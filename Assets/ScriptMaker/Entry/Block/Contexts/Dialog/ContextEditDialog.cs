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
    public abstract class ContextEditDialog: UI
    {
        protected readonly InputField.OnValidateInput floatValidator = (text, index, addedChar) =>
            addedChar is not '.' and < '0' or > '9'
                ? '\0'
                : (addedChar == '.' && text.Contains('.') ? '\0' : addedChar);

        public EditEntryGui baseDialog;
        public Option Context { get; set; }
        public override bool IsFront => baseDialog.IsFront;

        public void Start()
        {
            SetState(65, -38);
            Initialize();
            ModEvent.Context.OpenDialogEvent(this);
        }
        
        protected virtual void Initialize() {}
        
        private float currentX = 0, currentY = 0, cmax = 0;
        
        public void SetState(float x, float y)
        {
            currentX = x;
            currentY = y;
            cmax = 0;
        }
        
        public void AddInputField(string name, UnityAction<string> onValueChanged, string text = "", float width = 1050, float height = 365,
            float xBias = 0, int fontSize = 18, UnityAction<InputField> transform = null)
        {
            CreateObject(out var g, name);
            var rect = g.AssignRectTransform();
            rect.anchorMax = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = new Vector2(currentX + xBias, currentY);
            
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

            cmax = Mathf.Max(cmax, height);

            transform?.Invoke(ipf);
            ModEvent.Context.CreateInputFieldEvent(this, ipf);
        }

        public void AddButton(string text, UnityAction onClick, float width = 200, float height = 50, float xBias = 0, int fontSize = 18, UnityAction<Button> transform = null)
        {
            var btn = CreateButton(out var g, text + "Button", onClick);
            var rect = g.GetComponent<RectTransform>();
            rect.anchorMax = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = new Vector2(currentX + xBias, currentY);
            
            CreateText(out var gt, "Text", text, g, Color.black, TextAnchor.MiddleCenter);
            gt.GetComponent<RectTransform>().localPosition = new Vector3(width / 2, -height / 2);
            gt.GetComponent<Text>().fontSize = fontSize;

            cmax = Mathf.Max(cmax, height);
            transform?.Invoke(btn);
            ModEvent.Context.CreateButtonEvent(this, btn);
        }

        public void AddSingleLineInputField(string name,
            UnityAction<string> onValueChanged, string text = "", float width = 1050, float height = 30,
            float xBias = 0, int fontSize = 18, Action<InputField> transform = null)
        {
            CreateObject(out var g, name);
            var rect = g.AssignRectTransform();
            rect.anchorMax = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = new Vector2(currentX + xBias, currentY);
            
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
            
            cmax = Mathf.Max(cmax, height);
            transform?.Invoke(ipf);
            ModEvent.Context.CreateInputFieldEvent(this, ipf);
        }

        public void AddText(string name, string text, float width = 150, float height = 25, float xBias = 0,
            int fontSize = 18, Action<Text> transform = null, Color? color = null)
        {
            color ??= Color.black;
            var t = CreateText(out var g, name, text, color, TextAnchor.MiddleCenter);
            t.fontSize = fontSize;
            
            var rect = g.GetComponent<RectTransform>();
            rect.anchorMax = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = new Vector2(currentX + xBias, currentY);
            
            cmax = Mathf.Max(cmax, height);
            transform?.Invoke(t);
            ModEvent.Context.CreateTextEvent(this, t);
        }

        public void AddDropdownMenu(string name, List<(string displayName, string serializedName)> menus, Action<string> onValueChanged,
            string defaultValue = null, float width = 300, float height = 60, float xBias = 0, int fontSize = 24,
            Action<Dropdown> transform = null)
        {
            var g = (Instantiate(Resources.Load("Dropdown"), this.transform, true) as GameObject)!;
            g.name = name;
            var rect = g.GetComponent<RectTransform>();
            rect.anchorMax = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = new Vector2(currentX + xBias, currentY);

            cmax = Mathf.Max(cmax, height);
            
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
            dropdown.value = dropdown.options.FindIndex(x => x.text == menus.First(x => x.serializedName == defaultValue).displayName);
            dropdown.onValueChanged.AddListener(x =>
            {
                var k = menus.First(x2 => x2.displayName == dropdown.options[x].text)
                    .serializedName;
                onValueChanged?.Invoke(k);
                ModEvent.Context.OnDropdownMenuChangedEvent(this, dropdown, k);
            });
        }

        public void Next(float delta = 10) => currentY -= cmax + delta;

        public void AddImage(string name, float width, float height, float xBias = 0, Action<Image> transform = null)
        {
            var x = CreateImage(out var g, name);
            var rect = g.GetComponent<RectTransform>();
            rect.anchorMax = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = new Vector2(currentX + xBias, currentY);

            cmax = Mathf.Max(cmax, height);

            transform?.Invoke(x);
            ModEvent.Context.CreateImageEvent(this, x);
        }

        public void AddCheckbox(string name, string text, UnityAction<bool> onValueChanged, bool defaultValue = false,
            float width = 320, float height = 40, float xBias = 0, Action<Toggle> transform = null)
        {
            var x = CreateCheckbox(out var g, name, text, defaultValue);
            var rect = g.GetComponent<RectTransform>();
            rect.anchorMax = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = new Vector2(currentX + xBias, currentY);

            cmax = Mathf.Max(cmax, height);
            
            x.onValueChanged.AddListener(onValueChanged);
            
            transform?.Invoke(x);
            ModEvent.Context.CreateCheckboxEvent(this, x);
        }
    }
}