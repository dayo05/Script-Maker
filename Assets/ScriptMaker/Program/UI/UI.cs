using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ScriptMaker.Program.UI
{
    public abstract class UI : UIBase
    {
        public virtual bool IsFront { get; set; }

        public virtual void OnClose()
        {
        }

        protected void AddDefaultUI()
        {
            CreateObject(out var background, "Background");
            background.AssignRectTransform();
            background.SetObjectSize(1280, 720);
            background.SetObjectDefaultPos();
            var bgi = background.AddComponent<Image>();
            bgi.color = Color.gray;

            CreateButton(out var closeButton, "CloseButton", () =>
            {
                if (IsFront)
                    UIManager.CloseGui();
            });
            closeButton.SetObjectSize(75, 75);
            closeButton.SetObjectPos(-550, 275);
            closeButton.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 45));
            closeButton.ButtonTransaction(button =>
            {
                button.transition = Selectable.Transition.SpriteSwap;
                var sprite = button.spriteState;
                var tex = Resources.Load("plus") as Texture2D;
                sprite.disabledSprite =
                    Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                sprite.highlightedSprite =
                    Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                sprite.pressedSprite =
                    Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                sprite.selectedSprite =
                    Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                button.spriteState = sprite;

                button.image.sprite =
                    Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            });
        }
    }

    public class UIBase : MonoBehaviour
    {
        protected void CreateObject(out GameObject g, string name)
        {
            g = new GameObject(name);
            g.transform.SetParent(transform);
        }

        protected void CreateObject(out GameObject g, string name, GameObject parent)
        {
            g = new GameObject(name);
            g.transform.SetParent(parent.transform);
        }

        protected Image CreateImage(out GameObject g, string name = "image")
        {
            return CreateImage(out g, gameObject, name);
        }

        protected Image CreateImage(out GameObject g, GameObject parent, string name = "image")
        {
            CreateObject(out g, name);
            return g.AddComponent<Image>();
        }

        protected Button CreateButton(out GameObject g, string name, UnityAction onClick)
        {
            return CreateButton(out g, name, gameObject, onClick);
        }

        protected Button CreateButton(out GameObject g, string name, GameObject parent, UnityAction onClick)
        {
            CreateObject(out g, name, parent);
            var btn = g.AddComponent<Button>();
            btn.image = g.AddComponent<Image>();
            btn.onClick.AddListener(onClick);
            return btn;
        }

        protected Text CreateText(out GameObject g, string name, string text, Color? color = null,
            TextAnchor? alignment = null)
        {
            return CreateText(out g, name, text, gameObject, color, alignment);
        }

        protected Text CreateText(out GameObject g, string name, string text, GameObject parent, Color? color = null,
            TextAnchor? alignment = null)
        {
            CreateObject(out g, name, parent);
            var t = g.AddComponent<Text>();
            t.text = text;
            t.font = Resources.Load("d2") as Font;
            t.fontSize = 24;
            t.color = color ?? Color.black;
            if (alignment is not null)
                t.alignment = alignment.Value;
            return t;
        }

        protected InputField CreateInputField(out GameObject g, string name, string defaultText = "",
            bool withDescription = false)
        {
            return CreateInputField(out g, name, gameObject, defaultText, withDescription);
        }

        protected InputField CreateInputField(out GameObject g, string name, GameObject parent, string defaultText = "",
            bool withDescription = false)
        {
            CreateObject(out g, name, parent);
            g.AssignRectTransform();
            if (withDescription)
            {
                CreateText(out var desc, name, name, g, alignment: TextAnchor.MiddleCenter);
                desc.SetObjectSize(100, 42);
                desc.SetObjectDefaultPos();
                desc.TextTransaction(text => { text.fontSize = 19; });
            }

            CreateObject(out var inputFieldObject, "Input field", g);
            var ipf = inputFieldObject.AddComponent<InputField>();
            ipf.text = defaultText;
            ipf.image = inputFieldObject.AddComponent<Image>();
            inputFieldObject.SetObjectSize(200, 40);
            if (withDescription)
                inputFieldObject.SetObjectPos(150, 0);
            else inputFieldObject.SetObjectDefaultPos();

            var ipfText = CreateText(out var inputFieldTextObj, "Input field text", "", inputFieldObject);
            inputFieldTextObj.SetObjectDefaultPos();
            inputFieldTextObj.SetObjectSize(196, 36);
            ipfText.text = defaultText;
            ipf.textComponent = inputFieldTextObj.GetComponent<Text>();
            ipf.textComponent.supportRichText = false;
            return ipf;
        }

        protected Toggle CreateCheckbox(out GameObject g, string name, string text, bool defaultValue = false,
            GameObject parent = null)
        {
            parent ??= gameObject;
            g = Instantiate(Resources.Load("Checkbox"), parent.transform, true) as GameObject;
            g.name = name;
            var toggle = g.GetComponent<Toggle>();
            toggle.isOn = defaultValue;

            g.transform.Find("Label").GetComponent<Text>().text = text;
            return toggle;
        }
    }

    public static class UIExtension
    {
        public static void TextTransaction(this GameObject g, Action<Text> action)
        {
            action(g.GetComponent<Text>());
        }

        public static void SetButtonColor(this GameObject g, Func<ColorBlock, ColorBlock> transaction)
        {
            g.GetComponent<Button>().colors = transaction(g.GetComponent<Button>().colors);
        }

        public static void SetObjectSize(this GameObject g, float width, float height)
        {
            g.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        }

        public static void SetObjectPos(this GameObject g, float x, float y, float z = 0)
        {
            g.GetComponent<RectTransform>().localPosition = new Vector3(x, y, z);
        }

        public static void SetObjectDefaultPos(this GameObject g)
        {
            g.GetComponent<RectTransform>().localPosition = Vector3.zero;
        }

        public static RectTransform AssignRectTransform(this GameObject g)
        {
            return g.AddComponent<RectTransform>();
        }

        public static void ButtonTransaction(this GameObject g, Action<Button> action)
        {
            action(g.GetComponent<Button>());
        }
    }
}