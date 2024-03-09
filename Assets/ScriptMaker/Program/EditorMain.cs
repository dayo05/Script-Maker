using System;
using System.Diagnostics;
using ScriptMaker.Entry.Arrow;
using ScriptMaker.Entry.Arrow.Contexts;
using ScriptMaker.Entry.Block;
using ScriptMaker.Entry.Block.Contexts;
using ScriptMaker.Program.Data;
using ScriptMaker.Program.Mod;
using ScriptMaker.Program.UI;
using ScriptMaker.Program.UI.RightClickMenu;
using ScriptMaker.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ScriptMaker.Program
{
    public class EditorMain : UIBase
    {
        public static EditorMain Instance;

        private static GameObject ui;
        private static GameObject mainCamera;
        private static GameObject modInfoText;
        public static long PointedNS = -1;

        private static long currentOpenNs = -1;
        private static long copied = -1;
        public static bool IsArrowSelectionMod;

        private static long fromSel = -1;

        public static long currentNS = 0;
        private readonly float uiYPosBL = 70;
        private bool closeHandle;

        private float uiYPosTL = -70;

        public static bool IsIgnoreSelectionMod =>
            IsArrowSelectionMod || UIManager.IsGuiExists();

        private void Awake()
        {
            if (Instance is not null)
                throw new InvalidOperationException("Cannot initialize EditorMain twice");
            Instance = this;

            Log.Info("Start initializing editor");

            ui = GameObject.Find("UI");

            Application.logMessageReceived += (condition, trace, type) =>
            {
                if (type != LogType.Exception) return;
                Log.Error($"Exception thrown: {condition}");
                Log.Error($"Stack trace: ${trace}");
            };

            Log.Info("Try initialize managers");
            UIManager.Initialize();
            RightClickMenuHandler.Initialize();
            ArrowHandler.Initialize();
            FileManager.Initialize();
            BlockHandler.Initialize();
            BlockHandler.RegisterBlock(typeof(BeginBlock), new Option("Context", "BeginBlockContext")
                .Append("Label", "default"), typeof(BeginBlockContextEditDialog), "Begin");

            InitializeUI();

            Log.Info("Start loading mods");
            ModManager.LoadMods();

            modInfoText = GameObject.Find("ModInfoText");
            SetModInfoText("일반 모드");
        }

        private void Start()
        {
            BlockHandler.InitializeBeginEntry();

            Log.Info("Initializing editor finished");
            Log.Info("Running Post Initialization Step");
            ModEvent.Context.PostLoadingEvent();
        }

        private void Update()
        {
            ProcessKeyInput();
        }

        public void AddUIButtonTL(string name, Texture2D texture, UnityAction onClick)
        {
            CreateButton(out var addBtn, name, ui, onClick);
            var im = addBtn.GetComponent<Image>();
            im.sprite = Sprite.Create(texture, Rect.MinMaxRect(0, 0, texture.width, texture.height), Vector2.zero);
            var rect = addBtn.GetComponent<RectTransform>();
            rect.anchorMax = new Vector2(0, 1);
            rect.anchorMin = new Vector2(0, 1);
            rect.anchoredPosition = new Vector3(70, uiYPosTL);
            rect.sizeDelta = new Vector2(100, 100);
            uiYPosTL -= 110;
        }

        public void AddUIButtonBL(string name, Texture2D texture, UnityAction onClick)
        {
            CreateButton(out var addBtn, name, ui, onClick);
            var im = addBtn.GetComponent<Image>();
            im.sprite = Sprite.Create(texture, Rect.MinMaxRect(0, 0, texture.width, texture.height), Vector2.zero);
            var rect = addBtn.GetComponent<RectTransform>();
            rect.anchorMax = new Vector2(0, 0);
            rect.anchorMin = new Vector2(0, 0);
            rect.anchoredPosition = new Vector3(70, uiYPosBL);
            rect.sizeDelta = new Vector2(100, 100);
            uiYPosTL += 110;
        }

        private void InitializeUI()
        {
            var addThing = GameObject.Find("AddThing");
            addThing.SetActive(false);
            AddUIButtonTL("AddButton", Resources.Load("Images/plus") as Texture2D, () =>
            {
                if (IsIgnoreSelectionMod) return;
                addThing.SetActive(!addThing.activeSelf);
            });
            AddUIButtonTL("OpenButton", Resources.Load("Images/open") as Texture2D, () =>
            {
                if (IsIgnoreSelectionMod) return;
                FileManager.LoadFile();
            });
            AddUIButtonTL("SaveButton", Resources.Load("Images/save") as Texture2D, () =>
            {
                if (IsIgnoreSelectionMod) return;
                FileManager.SaveFile();
                ReCalcDisplayUI("저장되었습니다.");
            });
            AddUIButtonTL("CreateNewButton", Resources.Load("Images/create_new") as Texture2D, () =>
            {
                if (IsIgnoreSelectionMod) return;
                Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            });

            AddUIButtonBL("GlobalSettingsButton", Resources.Load("Images/settings") as Texture2D, () =>
            {
                if (IsIgnoreSelectionMod) return;
                UIManager.DisplayGui(typeof(ScriptSettingsGui));
            });

            foreach (var x in addThing.GetComponentsInChildren<Button>())
                switch (x.name)
                {
                    case "AddBlockButton":
                        x.onClick.AddListener(() =>
                        {
                            if (IsIgnoreSelectionMod) return;
                            addThing.SetActive(false);
                            BlockHandler.CreateEntryWithDialog();
                        });
                        break;
                    case "AddArrowButton":
                        x.onClick.AddListener(() =>
                        {
                            if (IsIgnoreSelectionMod) return;
                            addThing.SetActive(false);
                            StartCreateNewArrow();
                        });
                        break;
                }

            Application.wantsToQuit += () =>
            {
                if (closeHandle) return true;
                DialogGui.DisplayDialog("저장할까요?\n저장되지 않은 모든 데이터는 사라집니다.", DialogType.YesNoCancel, result =>
                {
                    switch (result)
                    {
                        case DialogResult.Yes:
                            FileManager.SaveFile(() =>
                            {
                                closeHandle = true;
                                Application.Quit();
                            });
                            break;
                        case DialogResult.No:
                            closeHandle = true;
                            Application.Quit();
                            break;
                        case DialogResult.Cancel:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(result), result, null);
                    }
                });
                return false;
            };
        }

        private static void SetModInfoText(string text)
        {
            modInfoText.GetComponent<Text>().text = text;
        }

        private void ProcessKeyInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!RightClickMenuHandler.IsPointerHoverMenu())
                    RightClickMenuHandler.CloseMenu();
                if (BlockHandler.IsNSExists(PointedNS))
                {
                    currentOpenNs = PointedNS;
                    var entry = BlockHandler.Blocks[PointedNS];
                    if (IsIgnoreSelectionMod)
                    {
                        if (IsArrowSelectionMod)
                        {
                            SetArrowLocation(entry.NS);
                            entry.transform.SetAsLastSibling();

                            currentOpenNs = entry.NS;
                        }

                        return;
                    }

                    ReCalcDisplayUI($"{entry.NS}번 블럭 선택됨");
                    entry.transform.SetAsLastSibling();

                    currentOpenNs = entry.NS;
                }
                else if (ArrowHandler.IsNSExists(PointedNS))
                {
                    var entry = ArrowHandler.Arrows[PointedNS];
                    if (IsIgnoreSelectionMod) return;
                    currentOpenNs = -1;
                    ReCalcDisplayUI(ArrowHandler.GetArrowContext(entry.NS).DisplayName(entry.NS));
                }
                else
                {
                    if (!IsIgnoreSelectionMod)
                        ReCalcDisplayUI();
                    else ReCalcDisplayUIWithKeepPrevInfo();
                    currentOpenNs = -1;
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (BlockHandler.IsNSExists(PointedNS))
                    RightClickMenuHandler.OpenMenu(typeof(BlockRightClickMenu), PointedNS);
                else if (ArrowHandler.IsNSExists(PointedNS))
                    RightClickMenuHandler.OpenMenu(typeof(ArrowRightClickMenu), PointedNS);
            }
            else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
            {
                if (currentOpenNs != -1)
                    copied = currentOpenNs;
            }
            else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
            {
                if (copied != -1)
                    BlockHandler.CreateEntry(BlockHandler.GetBlockContent(copied).Clone(),
                        new Point(BlockHandler.Blocks[copied].Point.X + 10, BlockHandler.Blocks[copied].Point.Y - 10));
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (IsArrowSelectionMod && !UIManager.IsGuiExists())
                {
                    IsArrowSelectionMod = false;
                    fromSel = -1;
                    ReCalcDisplayUI();
                }
            }
        }

        public static void StartCreateNewArrow()
        {
            IsArrowSelectionMod = true;
            ReCalcDisplayUI("화살표 생성 모드");
            fromSel = -1;
        }

        private static void SetArrowLocation(long NS)
        {
            if (fromSel == -1)
            {
                fromSel = NS;
                ReCalcDisplayUI($"화살표 생성 모드: {NS}번 블럭 선택됨");
            }
            else if (fromSel == NS)
            {
                fromSel = -1;
                ReCalcDisplayUI("화살표 생성 모드");
            }
            else
            {
                ArrowHandler.CreateNewArrow(new ArrowContext
                {
                    From = fromSel,
                    To = NS
                });
                fromSel = -1;
            }
        }

        public static void ReCalcDisplayUIWithKeepPrevInfo()
        {
            ui.SetActive(!IsIgnoreSelectionMod);
        }

        public static void ReCalcDisplayUI(string data = "")
        {
            if (IsIgnoreSelectionMod)
            {
                ui.SetActive(false);
            }
            else
            {
                ui.SetActive(true);
                if (data == "")
                    data = "일반 모드";
            }

            SetModInfoText(data);
        }

        public static void DeselectEntry()
        {
            currentOpenNs = -1;
        }
    }
}