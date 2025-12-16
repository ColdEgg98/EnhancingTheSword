using UnityEngine;

public class GetToastTest : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.uiManager.popupUIFactory.ShowToast("임시 아이템");
        GameManager.Instance.uiManager.popupUIFactory.ShowToast("임시 아이템");
        GameManager.Instance.uiManager.popupUIFactory.ShowToast("임시 아이템");
    }
}
