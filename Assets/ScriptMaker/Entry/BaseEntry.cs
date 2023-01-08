using System;
using ScriptMaker.Entry.Arrow;
using ScriptMaker.Entry.Block;
using ScriptMaker.Program;
using UnityEngine;

namespace ScriptMaker.Entry
{
    public abstract class BaseEntry: MonoBehaviour
    {
        public long NS { get; set; } = -1;
        
        public static void DeleteContent(long NS)
        {
            if (BlockHandler.IsNSExists(NS))
            {
                BlockHandler.RemoveBlock(NS);
                EditorMain.DeselectEntry();
            }
            else if (ArrowHandler.IsNSExists(NS))
            {
                ArrowHandler.RemoveArrow(NS);
                EditorMain.DeselectEntry();
            }
            else throw new IndexOutOfRangeException($"Trying to remove not exists namespace {NS}");
        }
    }
}