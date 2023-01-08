using ScriptMaker.Entry.Block;
using ScriptMaker.Util;
using UnityEngine;

namespace ScriptMaker.Entry.Arrow
{
    public class Arrow: BaseArrow
    {
        private GameObject triangle;
        protected void Start()
        {
            triangle = Instantiate(Resources.Load("Triangle") as GameObject, GameObject.Find("Canvas").transform, true);
        }

        protected override void Update()
        {
            base.Update();
            transform.SetAsFirstSibling();
            var v1 = new Vector3(BlockHandler.Blocks[Context.From].Point.X, BlockHandler.Blocks[Context.From].Point.Y, 0);
            var v2 = new Vector3(BlockHandler.Blocks[Context.To].Point.X, BlockHandler.Blocks[Context.To].Point.Y, 0);
            triangle.transform.localPosition = (v1, v2).Mean();
            var t = v2 - v1;
            triangle.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(t.y, t.x) * Mathf.Rad2Deg - 90);
        }

        protected void OnDestroy()
        {
            Destroy(triangle);
        }
    }
}