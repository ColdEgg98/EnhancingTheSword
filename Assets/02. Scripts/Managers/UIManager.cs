using UnityEngine;

public class UIManager : MonoBehaviour
{
    public UIFactory UIFactory;
    // saveSlot에서 호출
    public void Init()
    {
        if (UIFactory == null)
            UIFactory = FindAnyObjectByType<UIFactory>();
    }
}
    //사용법 : GameManager.Instance.uiManager.popupUIFactory.ShowToast(mesage);
