using UnityEngine;

namespace ScriptMaker.Program
{
    public class MainCameraCS : MonoBehaviour
    {
        void Update()
        {
            if (EditorMain.IsIgnoreSelectionMod) return;
            
            if (Input.GetKey(KeyCode.A))
                transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime);
            if (Input.GetKey(KeyCode.S))
                transform.Translate(new Vector3(0, -1, 0) * Time.deltaTime);
            if (Input.GetKey(KeyCode.D))
                transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime);
            if (Input.GetKey(KeyCode.W))
                transform.Translate(new Vector3(0, 1, 0) * Time.deltaTime);
        }
    }
}
