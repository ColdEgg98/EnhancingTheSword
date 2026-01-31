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
        GameManager.Instance.userDataManager.GetGold(-10000);
        GameManager.Instance.userDataManager.GetWeapon(1);
    }
}
