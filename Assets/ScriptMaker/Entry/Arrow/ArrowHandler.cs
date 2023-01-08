using System;
using System.Collections.Generic;
using System.Linq;
using ScriptMaker.Entry.Arrow.Contexts;
using ScriptMaker.Entry.Block;
using ScriptMaker.Entry.Block.Interface;
using ScriptMaker.Program;
using ScriptMaker.Program.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ScriptMaker.Entry.Arrow
{
    public static class ArrowHandler
    {
        public static readonly Dictionary<long, BaseArrow> Arrows = new();
        private static GameObject ArrowEntry;
        private static GameObject Canvas;

        public static void Initialize()
        {
            ArrowEntry = Resources.Load("Arrow") as GameObject;
            Canvas = GameObject.Find("Canvas");
        }

        public static bool IsNSExists(long NS) => Arrows.ContainsKey(NS);
        public static BaseArrowContext GetArrowContext(long NS) => Arrows[NS].Context;

        public static void RemoveArrow(long NS)
        {
            Object.Destroy(Arrows[NS].gameObject);
            Arrows.Remove(NS);
        }
        public static void TrySetArrowKey(long NS, string key)
        {
            try
            {
                if (Arrows[NS].Context is not ArrowContext)
                    throw new AccessViolationException("Not able to set key on this Arrow");
                var ctx = (ArrowContext)Arrows[NS].Context;
                ValidationArrowKey(ctx, key);
                ctx.Key = key;
                EditorMain.IsSelectingKeyOfArrow = false;
                EditorMain.ReCalcDisplayUI();
            }
            catch (InvalidOperationException)
            {
                EditorMain.ReCalcDisplayUI("키 조합이 중복됩니다!");
            }
        }
        
        private static void ValidationNewArrow(BaseArrowContext arrow)
        {
            if (BlockHandler.Blocks[arrow.To] is BeginBlock)
                throw new InvalidOperationException("Begin entry is not able to receive arrow");
            var cached = Arrows.Where(x => x.Value.Context.From == arrow.From).ToArray();
            if (cached.Any(x => BlockHandler.Blocks[x.Value.Context.To] is not MultipleSelectableBlock) || cached.Length != 0 && BlockHandler.Blocks[arrow.To] is not MultipleSelectableBlock)
                throw new InvalidOperationException("2+ child is only allowed when every mod of child is Player");
        }

        private static void ValidationArrowKey(ArrowContext arrow, string newKey)
        {
            if (Arrows.Where(x => x.Value.Context is ArrowContext && x.Value.Context.From == arrow.From).Any(x => ((ArrowContext)x.Value.Context).Key == newKey && x.Value.Context.To != arrow.To))
                throw new InvalidOperationException("Arrow key is not allowed to duplicate in one starting block.");
        }
        
        public static void CreateArrowUnsafe(long NS, ArrowContext a)
        {
            var g = Object.Instantiate(ArrowEntry, Canvas.transform, true);
            var arrow = g.AddComponent<ScriptMaker.Entry.Arrow.Arrow>();
            arrow.NS = NS;
            arrow.Context = a;
            Arrows.Add(NS, arrow);
        }

        public static void CreateNewArrow(ArrowContext a)
        {
            try
            {
                ValidationNewArrow(a);
                CreateArrowUnsafe(EditorMain.currentNS++, a);
                EditorMain.IsArrowSelectionMod = false;
                EditorMain.ReCalcDisplayUI();
            }
            catch (Exception e)
            {
                DialogGui.DisplayDialog(e.Message);
            }
            finally
            {
                if(EditorMain.IsArrowSelectionMod)
                    EditorMain.ReCalcDisplayUI("화살표 생성 모드");
            }
        }
    }
}