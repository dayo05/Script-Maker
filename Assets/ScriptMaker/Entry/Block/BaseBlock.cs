using System;
using ScriptMaker.Entry.Block.Contexts;
using ScriptMaker.Program;
using ScriptMaker.Program.Data;
using ScriptMaker.Program.UI;
using ScriptMaker.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ScriptMaker.Entry.Block
{
    public abstract class BaseBlock: BaseEntry, IDragHandler, IBeginDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private GameObject mainCamera;
        private GameObject content;
        private GameObject innerText;

        public Point Point = new();

        protected virtual void Start()
        {
            mainCamera = GameObject.FindWithTag("MainCamera");
            transform.localPosition = new Vector3(Point.X, Point.Y, 0);
            content = transform.Find("Content").gameObject;
            innerText = content.transform.Find("InnerText").gameObject;
            
            innerText.GetComponent<Text>().text = Text;
            content.GetComponent<Image>().color = Color;
        }
        
        protected abstract string Text { get; }
        protected abstract Color Color { get; }

        public Option Context { get; set; }

        protected virtual bool ValidateBlock(long NS, Option c) => true;

        private void Initialize(long NS, Option c)
        {
            if (!ValidateBlock(NS, c))
            {
                DialogGui.DisplayDialog("Not able to initialize block.");
                throw new ArgumentException("Validation failed");
            }
            this.Context = c;
            this.NS = NS;
        }

        public void Initialize(long NS, Option c, Point point)
        {
            Initialize(NS, c);
            this.Point = point;
        }

        private Vector3 deltaMov;

        public void OnDrag(PointerEventData eventData)
        {
            if (EditorMain.IsIgnoreSelectionMod) return;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(this.transform.parent.GetComponent<RectTransform>(),
                eventData.position, mainCamera.GetComponent<Camera>(), out var o);
            this.transform.position = o - deltaMov;
            BlockHandler.Blocks[NS].Point.X = GetComponent<RectTransform>().localPosition.x;
            BlockHandler.Blocks[NS].Point.Y = GetComponent<RectTransform>().localPosition.y;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (EditorMain.IsIgnoreSelectionMod) return;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(this.transform.parent.GetComponent<RectTransform>(),
                eventData.position, mainCamera.GetComponent<Camera>(), out var o);
            deltaMov = o - transform.position;
            transform.SetAsLastSibling();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            EditorMain.PointedNS = NS;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (EditorMain.PointedNS == NS)
                EditorMain.PointedNS = -1;
        }

        public virtual Option Serialize() => new Option("Block", this.GetType().Name)
            .Append("NS", NS)
            .Append("X", Point.X)
            .Append("Y", Point.Y)
            .Append(Context);
    }
}