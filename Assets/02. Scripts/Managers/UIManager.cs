using UnityEngine;

public class UIManager : MonoBehaviour
{
    public PopUpUIFactory popupUIFactory;
    // saveSlot에서 호출
    public void Init()
    {
        if (popupUIFactory == null)
            popupUIFactory = FindAnyObjectByType<PopUpUIFactory>();
    }
}
    //사용법 : GameManager.Instance.uiManager.popupUIFactory.ShowToast(mesage);
