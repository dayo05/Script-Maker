using System;
using System.Linq;
using ScriptMaker.Entry.Block;
using ScriptMaker.Entry.Block.Contexts;
using ScriptMaker.Entry.Block.Contexts.Dialog;
using UnityEngine;
using UnityEngine.UI;

namespace ScriptMaker.Program.UI
{
    public class EditEntryGui: UI
    {
        public long NS { get; set; }

        void Start()
        {
            AddDefaultUI();
            var contexts = BlockHandler.handler.Where(x => x.Value.DisplayName is not null).ToList();
            
            CreateObject(out var subOption, "SubOption");
            subOption.AssignRectTransform();
            subOption.SetObjectSize(1180, 475);
            subOption.SetObjectPos(0, -25);
            if (NS != -1)
            {
                var ed = (ContextEditDialog) subOption.AddComponent(
                    BlockHandler.GetDialog(BlockHandler.GetBlockContent(NS).Value));
                ed.Context = BlockHandler.GetBlockContent(NS).Clone();
                ed.baseDialog = this;
            }
            else
            {
                var ed = (ContextEditDialog) subOption.AddComponent(contexts[0].Value.EditDialog);
                ed.Context = BlockHandler.handler[contexts[0].Key].DefaultContext.Clone();
                ed.baseDialog = this;
            }

            CreateButton(out var applyButton, "ApplyButton", () =>
            {
                if (!IsFront) return;
                if (NS == -1)
                    BlockHandler.CreateEntry(subOption.GetComponent<ContextEditDialog>().Context);
                else BlockHandler.InplaceEntry(NS, subOption.GetComponent<ContextEditDialog>().Context);
                UIManager.CloseGui();
            });
            applyButton.SetObjectPos(400, -315);
            applyButton.SetObjectSize(200, 50);
            CreateText(out var applyButtonTextObj, "ApplyButtonText", "Apply", applyButton, Color.black,
                TextAnchor.MiddleCenter);
            applyButtonTextObj.SetObjectDefaultPos();
            applyButtonTextObj.GetComponent<Text>().fontSize = 24;

            var dropdownObj = Instantiate(Resources.Load("Dropdown"), this.transform, true) as GameObject;
            dropdownObj.SetObjectPos(-300, 275);
            dropdownObj.SetObjectSize(400, 60);
            dropdownObj.transform.Find("Label").GetComponentInChildren<Text>().fontSize = 32;

            {
                var template = dropdownObj.transform.Find("Template").Find("Viewport").Find("Content");
                template.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 56);
                var item = template.Find("Item");
                item.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 40);
                var label = item.Find("Item Label");
                label.GetComponent<Text>().fontSize = 24;
            }

            var dropdown = dropdownObj.GetComponent<Dropdown>();
            dropdown.AddOptions(contexts.Select(x => x.Value.DisplayName).ToList());
            if(NS != -1)
                dropdown.value = dropdown.options.FindIndex(x =>
                    x.text == BlockHandler.handler.First(x => x.Key == BlockHandler.GetBlockContent(NS).Value).Value
                        .DisplayName);
            dropdown.onValueChanged.AddListener(opt =>
            {
                Destroy(subOption);
                CreateObject(out subOption, "SubOption");
                subOption.AssignRectTransform();
                subOption.SetObjectSize(1180, 475);
                subOption.SetObjectPos(0, -25);
                var ed = (ContextEditDialog) subOption.AddComponent(BlockHandler.GetDialog(contexts[opt].Key));
                ed.Context = BlockHandler.handler[contexts[opt].Key].DefaultContext.Clone();
                ed.baseDialog = this;
            });
        }

        public static void OpenEditDialog(long NS)
        {
            Debug.Log("Try open " + NS);
            var ui = UIManager.DisplayGui(typeof(EditEntryGui)) as EditEntryGui;
            ui!.NS = NS;
        }
    }
}