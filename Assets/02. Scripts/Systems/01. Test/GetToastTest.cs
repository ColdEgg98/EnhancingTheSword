using UnityEngine;
using UnityEngine.UI;

public class GetToastTest : MonoBehaviour
{
    private Button btn;
    public string mesage = string.Empty;

    void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(ShowToast);
    }

    private void ShowToast()
    {
        GameManager.Instance.uiManager.UIFactory.ShowToast(mesage);
    }
}
