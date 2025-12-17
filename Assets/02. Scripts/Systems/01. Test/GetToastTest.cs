using UnityEngine;
using UnityEngine.UI;

public class GetToastTest : MonoBehaviour
{
    private Button btn;
    public string mesage = "테스트";

    void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(ShowToast);
    }

    private void ShowToast()
    {
        GameManager.Instance.uiManager.popupUIFactory.ShowToast(mesage);
    }
}
