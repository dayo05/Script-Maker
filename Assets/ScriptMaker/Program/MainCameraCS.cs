using System;
using UnityEngine;

namespace ScriptMaker.Program
{
    public class MainCameraCS : MonoBehaviour
    {
        private RectTransform MainCanvasTransform;

        private void Start()
        {
            MainCanvasTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();
        }

        private float ScaleBias => 30 * Time.deltaTime;

        void Update()
        {
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) && Input.mouseScrollDelta.normalized != Vector2.zero)
            {
                var ls = MainCanvasTransform.localScale;
                ls.Scale(Vector2.one * Mathf.Pow(1.1f, Input.mouseScrollDelta.y));
                if (ls.x > 0.15) ls = new Vector3(0.15f, 0.15f, 0.15f); // Clamp
                else if (ls.x < 0.001) ls = new Vector3(0.001f, 0.001f, 0.001f);
                MainCanvasTransform.localScale = ls;
            }
            else transform.Translate(Input.mouseScrollDelta * new Vector2(-1, 9.0f / 16));

            if (EditorMain.IsIgnoreSelectionMod) return;

            if (Input.GetKey(KeyCode.A))
                transform.Translate(new Vector3(-1, 0, 0) * ScaleBias);
            if (Input.GetKey(KeyCode.S))
                transform.Translate(new Vector3(0, -1, 0) * ScaleBias);
            if (Input.GetKey(KeyCode.D))
                transform.Translate(new Vector3(1, 0, 0) * ScaleBias);
            if (Input.GetKey(KeyCode.W))
                transform.Translate(new Vector3(0, 1, 0) * ScaleBias);
        }
    }
}