using System;
using ScriptMaker.Program;
using ScriptMaker.Program.Data;
using ScriptMaker.Program.UI;
using ScriptMaker.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ScriptMaker.Entry.Block
{
    public abstract class BaseBlock : BaseEntry, IDragHandler, IBeginDragHandler, IPointerEnterHandler,
        IPointerExitHandler
    {
        private GameObject content;

        private Vector3 deltaMov;
        private GameObject innerText;
        private GameObject mainCamera;

        public Point Point = new();

        protected abstract string Text { get; }
        protected abstract Color Color { get; }

        public Option Context { get; set; }

        protected virtual void Start()
        {
            mainCamera = GameObject.FindWithTag("MainCamera");
            transform.localPosition = new Vector3(Point.X, Point.Y, 0);
            content = transform.Find("Content").gameObject;
            innerText = content.transform.Find("InnerText").gameObject;

            innerText.GetComponent<Text>().text = Text;
            content.GetComponent<Image>().color = Color;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (EditorMain.IsIgnoreSelectionMod) return;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(transform.parent.GetComponent<RectTransform>(),
                eventData.position, mainCamera.GetComponent<Camera>(), out var o);
            deltaMov = o - transform.position;
            transform.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (EditorMain.IsIgnoreSelectionMod) return;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(transform.parent.GetComponent<RectTransform>(),
                eventData.position, mainCamera.GetComponent<Camera>(), out var o);
            transform.position = o - deltaMov;
            BlockHandler.Blocks[NS].Point.X = GetComponent<RectTransform>().localPosition.x;
            BlockHandler.Blocks[NS].Point.Y = GetComponent<RectTransform>().localPosition.y;
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

        protected virtual bool ValidateBlock(long NS, Option c)
        {
            return true;
        }

        private void Initialize(long NS, Option c)
        {
            if (!ValidateBlock(NS, c))
            {
                DialogGui.DisplayDialog("Not able to initialize block.");
                throw new ArgumentException("Validation failed");
            }

            Context = c;
            this.NS = NS;
        }

        public void Initialize(long NS, Option c, Point point)
        {
            Initialize(NS, c);
            Point = point;
        }

        public virtual Option Serialize()
        {
            return new Option("Block", GetType().Name)
                .Append("NS", NS)
                .Append("X", Point.X)
                .Append("Y", Point.Y)
                .Append(Context);
        }
    }
}