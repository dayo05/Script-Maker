using ScriptMaker.Program.UI;
using UnityEngine;

namespace ScriptMaker.Program
{
    public class MainCameraCS : MonoBehaviour
    {
        private Transform cameraTransform;
        private RectTransform MainCanvasTransform;
        private Camera self;

        private float ScaleBias => 30 * Time.deltaTime;

        private void Start()
        {
            MainCanvasTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();
            self = GetComponent<Camera>();
            cameraTransform = gameObject.transform;
        }

        private void Update()
        {
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) &&
                Input.mouseScrollDelta.normalized != Vector2.zero)
            {
                var ps = MainCanvasTransform.localScale.x;
                var ls = ps * Mathf.Pow(1.1f, Input.mouseScrollDelta.y);
                ls = Mathf.Clamp(ls, 0.001f, 0.15f);
                MainCanvasTransform.localScale = Vector3.one * ls;
                var dup = ls / ps;

                var bias = cameraTransform.position - self.ScreenToWorldPoint(Input.mousePosition);

                var lp = cameraTransform.localPosition;
                cameraTransform.localPosition = (lp - bias) * dup + bias;
                /*
                var ls = MainCanvasTransform.localScale;
                var dupCac = ls.x;
                ls.Scale(Vector2.one * Mathf.Pow(1.1f, Input.mouseScrollDelta.y));
                if (ls.x > 0.15) ls = new Vector3(0.15f, 0.15f, 0.15f); // Clamp
                else if (ls.x < 0.001) ls = new Vector3(0.001f, 0.001f, 0.001f);
                MainCanvasTransform.localScale = ls;
                var incrScale = ls.x / dupCac;
                transform.localPosition *= incrScale;
                */
            }
            else if (!UIManager.IsGuiExists())
            {
                cameraTransform.Translate(Input.mouseScrollDelta * new Vector2(-1, 9.0f / 16));
            }

            if (EditorMain.IsIgnoreSelectionMod) return;

            if (Input.GetKey(KeyCode.A))
                cameraTransform.Translate(new Vector3(-1, 0, 0) * ScaleBias);
            if (Input.GetKey(KeyCode.S))
                cameraTransform.Translate(new Vector3(0, -1, 0) * ScaleBias);
            if (Input.GetKey(KeyCode.D))
                cameraTransform.Translate(new Vector3(1, 0, 0) * ScaleBias);
            if (Input.GetKey(KeyCode.W))
                cameraTransform.Translate(new Vector3(0, 1, 0) * ScaleBias);
        }
    }
}