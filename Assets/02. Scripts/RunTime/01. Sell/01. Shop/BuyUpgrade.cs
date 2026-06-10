using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 상점 객체에 string으로 itemID를 기술하면 그 ID기반으로 업그레이드 진행
/// </summary>
public class BuyUpgrade : MonoBehaviour
{
    [SerializeField] private string itemID;
    private MaterialItem item;
    private Button button;
    private long Price => item.ItemPrice;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(TryPurchase);

        item = (MaterialItem)GameManager.Instance.allOfItemsDictionary[itemID];
    }

    private void TryPurchase()
    {
        if (!IsVaild()) return;

        item.action.Excute(item);
        GameManager.Instance.GetGold(-Price);
        GameManager.Instance.ShowNotice($"{StrUtiity.AttachJoSa(item.ItemName)} 완료 되었습니다!");
        // 모루 강화가 완료 되었습니다!
    }

    private bool IsVaild()
    {
        if (GameManager.Instance.gold.Value < Price)
        {
            GameManager.Instance.ShowNotice("골드가 부족합니다.");
            return false;
        }

        if (item.ActionString == "UpgradeAnvil" && GameManager.Instance.currentData.shopData.anvilLevel == 5)
        {
            GameManager.Instance.ShowNotice("이미 최대 레벨입니디.");
            return false;
        }

        if (item.ActionString == "UpgradeHammer" && GameManager.Instance.currentData.shopData.hammerLevel == 10)
        {
            GameManager.Instance.ShowNotice("이미 최대 레벨입니다.");
            return false;
        }

        return true;
    }
}
