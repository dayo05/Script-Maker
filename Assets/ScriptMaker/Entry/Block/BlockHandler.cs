using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ScriptMaker.Entry.Block.Contexts;
using ScriptMaker.Entry.Block.Contexts.Dialog;
using ScriptMaker.Program.Data;
using ScriptMaker.Program.Mod;
using ScriptMaker.Program.UI;
using ScriptMaker.Util;
using UnityEngine;
using static ScriptMaker.Program.EditorMain;
using Object = UnityEngine.Object;

namespace ScriptMaker.Entry.Block
{
    public class BlockInfo
    {
        public Type Block;
        public string DisplayName;
        public Type EditDialog;
        public Option DefaultContext;

        public void Validate()
        {
            if (!Block.IsSubclassOf(typeof(BaseBlock)))
                throw new ArgumentException($"Block {Block} don't extends BaseBlock");
            if (!EditDialog.IsSubclassOf(typeof(ContextEditDialog)))
                throw new ArgumentException($"Dialog {EditDialog} don't extends ContextEditDialog");
        }
    }
    public static class BlockHandler
    {
        public static Dictionary<string, BlockInfo> handler = new();
        public static Dictionary<long, BaseBlock> Blocks = new();
        
        private static GameObject BaseEntry;
        private static GameObject Canvas;
        private static GameObject MainCamera;

        public static void Initialize()
        {
            Canvas = GameObject.Find("Canvas");
            MainCamera = GameObject.Find("Main Camera");
            BaseEntry = Resources.Load("Entry") as GameObject;
        }
        
        public static Option GetBlockContent(long NS) => Blocks[NS].Context;

        public static bool IsNSExists(long NS) => Blocks.ContainsKey(NS);

        public static void RemoveBlock(long NS)
        {
            Object.Destroy(Blocks[NS].gameObject);
            Blocks.Remove(NS);
        }

        public static void RegisterBlock(Type block, Option defaultContext, Type contextEditDialogType, string displayName)
        {
            if (defaultContext.Key != "Context")
                throw new InvalidOperationException($"Not context option: {defaultContext.Key}");
            if (displayName == "Arrow")
                throw new ArgumentException("Invalid displayName");
            var info = new BlockInfo
            {
                Block = block,
                DisplayName = displayName,
                EditDialog = contextEditDialogType,
                DefaultContext = defaultContext
            };
            info.Validate();
            if (handler.ContainsKey(defaultContext.Value))
            {
                Log.Error(new ArgumentException($"Failed to Register block: {displayName}. Context {defaultContext.Value} already exists"), Assembly.GetCallingAssembly().GetName().Name);
                return;
            }

            handler[defaultContext.Value] = info;
            Log.Info($"Block registered: {displayName}", Assembly.GetCallingAssembly().GetName().Name);
        }
        
        private static BaseBlock CreateBlock(GameObject g, long NS, Option ctx, Point p)
        {
            if (ctx.Key != "Context")
                throw new InvalidOperationException($"Key of option must be Context, not {ctx.Key}");
            var b = g.AddComponent(handler[ctx.Value].Block) as BaseBlock;
            b.Initialize(NS, ctx, p);
            return b;
        }

        public static Type GetDialog(string ctxType)
            => handler[ctxType].EditDialog;

        public static GameObject InplaceEntry(long NS, Option c)
        {
            if (!Blocks.ContainsKey(NS))
                throw new Exception("Assertion Failed: !Blocks.ContainsKey(NS)");
            Object.Destroy(Blocks[NS].gameObject);

            return CreateEntryRaw(NS, c, new Point(Blocks[NS].Point.X, Blocks[NS].Point.Y));
        }

        public static GameObject CreateEntryByOption(Option option)
        {
            var ctxName = option["Context"].String();
            if (handler.Keys.All(x => x != ctxName))
                throw new ModLoadingException($"Block context {ctxName} not found.");
            return CreateEntryRaw(option["NS"].Long(),
                option["Context"].First(),
                new Point(option["X"].Float(), option["Y"].Float()));
        }

        public static GameObject CreateEntryRaw(long NS, Option c, Point p)
        {
            var g = Object.Instantiate(BaseEntry, Canvas.transform, true);
            Blocks[NS] = CreateBlock(g, NS, c, p);
            return g;
        }

        public static GameObject CreateEntry(Option c, Point p)
        {
            var pos = MainCamera.transform.localPosition;
            var g = CreateEntryRaw(currentNS++, c, p);
            g.transform.position = new Vector3(p.X, p.Y, pos.z);
            return g;
        }

        public static void InitializeBeginEntry()
        {
            CreateEntry(new Option("Context", "BeginBlockContext")
                .Append("Label", "default")).transform.localPosition = Vector3.zero;
        }

        public static GameObject CreateEntry(Option c)
        {
            var pos = MainCamera.transform.localPosition;
            return CreateEntry(c, new Point(pos.x, pos.y));
        }

        public static void CreateEntryWithDialog()
        {
            var gui = UIManager.DisplayGui(typeof(EditEntryGui)) as EditEntryGui;
            gui.NS = -1;
        }
    }
}