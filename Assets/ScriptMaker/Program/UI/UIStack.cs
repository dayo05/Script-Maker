using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace ScriptMaker.Program.UI
{
    public class InnerStack : MonoBehaviour
    {
        [CanBeNull] public UIStack parent;
        
        public float spaceUp = 0;
        public float spaceDown = 5;
        public float spaceLeft = 0;
        public float spaceRight = 5;

        public bool needsRecalculate { get; private set; } = false;

        public RectTransform SelfTransform { get; private set; }

        private Vector3 RectSize => new(SelfTransform.sizeDelta.x, SelfTransform.sizeDelta.y);

        public void SetHMargin(float sz)
        {
            spaceRight = spaceLeft = sz;
        }

        public void SetVMargin(float sz)
        {
            spaceUp = spaceDown = sz;
        }

        public void SetMargin(float sz)
        {
            SetHMargin(sz);
            SetVMargin(sz);
        }

        public virtual Vector3 SupplyLocation(Vector3 prev)
        {
            SelfTransform.localPosition = prev + new Vector3(spaceLeft, -spaceUp);
            var o = SelfTransform.localPosition + new Vector3(spaceRight + RectSize.x, -spaceDown - RectSize.y);
            return o;
        }

        public static InnerStack Create([CanBeNull] UIStack parent = null, string name = "")
        {
            return Create(new GameObject(name), parent);
        }

        public static InnerStack Create(GameObject selfObject, [CanBeNull] UIStack parent = null)
        {
            var iStack = selfObject.AddComponent<InnerStack>();
            iStack.ApplyRectTransform();
            
            if (parent is null) return iStack;
            selfObject.transform.SetParent(parent.transform);
            parent.Append(iStack);
            return iStack;
        }

        protected void ApplyRectTransform()
        {
            SelfTransform = gameObject.TryGetComponent<RectTransform>(out var rt)
                ? rt
                : gameObject.AddComponent<RectTransform>();
        }

        public void MarkDirty()
        {
            var t = this;
            while (t.parent is not null && !t.needsRecalculate) t = t.parent;
            t.needsRecalculate = true;
        }

        public bool IsVisible
        {
            get => gameObject.activeInHierarchy;
            set
            {
                gameObject.SetActive(value);
                MarkDirty();
            }
        }
    }

    public abstract class UIStack : InnerStack
    {
        private readonly List<InnerStack> innerStack = new();
        protected IEnumerable<InnerStack> Inner => innerStack.Where(x => x.isActiveAndEnabled);

        public void Append(InnerStack subStack)
        {
            innerStack.Add(subStack);
            subStack.transform.SetParent(transform);
            subStack.parent = this;
            MarkDirty();
        }

        public abstract void UpdateBatch();

        public override Vector3 SupplyLocation(Vector3 prev)
        {
            UpdateBatch();
            return base.SupplyLocation(prev);
        }
    }

    public class HStack : UIStack
    {
        public new static HStack Create([CanBeNull] UIStack parent = null, string name = "")
        {
            var selfObject = new GameObject(name);
            var hStack = selfObject.AddComponent<HStack>();
            hStack.ApplyRectTransform();
            
            hStack.SelfTransform.anchorMax = new Vector2(0, 1);
            hStack.SelfTransform.anchorMin = new Vector2(0, 1);
            hStack.SelfTransform.pivot = new Vector2(0, 1);
            
            if (parent is not null)
                parent.Append(hStack);

            return hStack;
        }

        public override void UpdateBatch()
        {
            var il = Vector3.zero;
            float hsz = 0;
            foreach (var sd in Inner.Select(i => i.SupplyLocation(il)))
            {
                hsz = Mathf.Min(hsz, sd.y);
                il = Vector3.right * sd.x;
            }

            SelfTransform.sizeDelta = il + hsz * Vector3.down;
        }
    }

    public class VStack : UIStack
    {
        public new static VStack Create([CanBeNull] UIStack parent = null, string name = "")
        {
            var selfObject = new GameObject(name);
            var vStack = selfObject.AddComponent<VStack>();
            vStack.ApplyRectTransform();
            
            vStack.SelfTransform.anchorMax = new Vector2(0, 1);
            vStack.SelfTransform.anchorMin = new Vector2(0, 1);
            vStack.SelfTransform.pivot = new Vector2(0, 1);
            
            if (parent is not null)
                parent.Append(vStack);
            
            return vStack;
        }

        public override void UpdateBatch()
        {
            var il = Vector3.zero;
            float vsz = 0;
            foreach (var sd in Inner.Select(i => i.SupplyLocation(il)))
            {
                vsz = Mathf.Max(vsz, sd.x);
                il = Vector3.up * sd.y;
            }

            SelfTransform.sizeDelta = Vector3.down * il.y + vsz * Vector3.right;
        }
    }
}