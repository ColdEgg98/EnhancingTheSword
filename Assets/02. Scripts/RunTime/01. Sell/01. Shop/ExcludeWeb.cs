using UnityEngine;
using UnityEngine.UI;

public class ExcludeWeb : MonoBehaviour
{
    private Button btn;

    void Start()
    {
        #if UNITY_WEBGL
        btn = GetComponent<Button>();
        btn.interactable = false;
        #endif
    }
}
