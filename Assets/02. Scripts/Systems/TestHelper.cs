using UnityEngine;
using UnityEngine.UI;

public class TestHelper : MonoBehaviour
{
    public Button btn;

    private void Awake()
    {
        #if UNITY_EDITOR
        Tester();
        #endif
    }

    public void Tester()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(TestButtonAction);
        gameObject.SetActive(true);
    }

    private void TestButtonAction()
    {
        
    }
}
