using UnityEngine;
using UnityEngine.UI;

public class BuyWoodSword : MonoBehaviour
{
    Button _btn;

    void Awake()
    {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(BuySword);
    }

    private void BuySword()
    {
        if (GameManager.Instance.currentData.myWeapons.Count < 20)
        {
            GameManager.Instance.userDataManager.GetGold(-10000);
            GameManager.Instance.userDataManager.GetWeapon(1);
        }
        else
        {
            GameManager.Instance.uiManager.UIFactory.ShowNotice("인벤토리가 가득 찼습니다.");
        }
    }
}
