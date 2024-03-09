using System.Collections.Generic;
using ScriptMaker.Util;
using UnityEngine;
using UnityEngine.Events;

namespace ScriptMaker.Program.UI.RightClickMenu
{
    public abstract class RightClickMenu : UIBase
    {
        protected const float buttonWidth = 141;
        protected const float buttonHeight = 35;

        private readonly List<GameObject> buttonList = new();

        public bool IsPointerOnMenu => Input.mousePosition.x.In(
                                           transform.position.x - GetComponent<RectTransform>().sizeDelta.x / 2,
                                           transform.position.x + GetComponent<RectTransform>().sizeDelta.x / 2) &&
                                       (Input.mousePosition.y - buttonHeight).In(
                                           transform.position.y - GetComponent<RectTransform>().sizeDelta.y -
                                           buttonHeight / 2,
                                           transform.position.y);

        public long NS { get; set; }

        protected virtual void Start()
        {
            gameObject.AddComponent<RectTransform>();
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                RightClickMenuHandler.CloseMenu();
        }

        public virtual void OnClose()
        {
        }

        protected GameObject AppendButton(string text, UnityAction action)
        {
            CreateButton(out var buttonObj, text, action);
            buttonObj.SetObjectSize(buttonWidth, buttonHeight);
            buttonObj.SetObjectPos(0, -buttonList.Count * buttonHeight);

            CreateText(out var buttonTextObj, text, text, buttonObj, Color.black, TextAnchor.MiddleCenter);
            buttonTextObj.SetObjectSize(buttonWidth, buttonHeight);
            buttonTextObj.SetObjectDefaultPos();

            buttonTextObj.TextTransaction(text => { text.fontSize = 24; });

            buttonList.Add(buttonObj);
            return buttonObj;
        }

        protected void LocateButton()
        {
            var baseRect = gameObject.GetComponent<RectTransform>();
            baseRect.sizeDelta = new Vector2(buttonWidth, buttonHeight * buttonList.Count);
            gameObject.transform.position = Input.mousePosition + (new Vector3(
                        baseRect.rect.width / 2,
                        -baseRect.rect.height / 2 + buttonHeight * (buttonList.Count - 1) / 2),
                    transform.localScale)
                .Eq();
        }
    }
}