using ScriptMaker.Entry.Arrow.Contexts;
using ScriptMaker.Entry.Block;
using ScriptMaker.Program;
using ScriptMaker.Program.Data;
using ScriptMaker.Util;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ScriptMaker.Entry.Arrow
{
    public class Arrow : BaseEntry, IPointerEnterHandler, IPointerExitHandler
    {
        public ArrowContext Context;
        private GameObject triangle;

        private void Start()
        {
            triangle = Instantiate(Resources.Load("Triangle") as GameObject, GameObject.Find("Canvas").transform, true);
        }

        protected void Update()
        {
            if (!BlockHandler.IsNSExists(Context.From) || !BlockHandler.IsNSExists(Context.To))
            {
                DeleteContent(NS);
                return;
            }

            {
                transform.SetAsFirstSibling();
                var v1 = new Vector3(BlockHandler.Blocks[Context.From].Point.X,
                    BlockHandler.Blocks[Context.From].Point.Y, 0);
                var v2 = new Vector3(BlockHandler.Blocks[Context.To].Point.X, BlockHandler.Blocks[Context.To].Point.Y,
                    0);
                transform.localPosition = (v1, v2).Mean();
                transform.localScale = new Vector3(Vector3.Distance(v1, v2) / 10, 1, 1);
                var t = v2 - v1;
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(t.y, t.x) * Mathf.Rad2Deg);
            }
            {
                transform.SetAsFirstSibling();
                var v1 = new Vector3(BlockHandler.Blocks[Context.From].Point.X,
                    BlockHandler.Blocks[Context.From].Point.Y, 0);
                var v2 = new Vector3(BlockHandler.Blocks[Context.To].Point.X, BlockHandler.Blocks[Context.To].Point.Y,
                    0);
                triangle.transform.localPosition = (v1, v2).Mean();
                var t = v2 - v1;
                triangle.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(t.y, t.x) * Mathf.Rad2Deg - 90);
            }
        }

        protected void OnDestroy()
        {
            Destroy(triangle);
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

        public Option Serialize()
        {
            return new Option("Arrow", GetType().Name)
                .Append("NS", NS)
                .Append(Context.Serialize());
        }
    }
}