using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 상점 객체에 string으로 itemID를 기술하면 그 ID기반으로 아이템 액션 진행
/// </summary>
public class RunShopItemAction : MonoBehaviour
{
    [SerializeField] private string itemID;
    private MaterialItem item;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(TryPurchase);

        // 아이템 정보 취득
        item = (MaterialItem)GameManager.Instance.allOfItemsDictionary[itemID];
    }

    private void TryPurchase()
    {
        // ItemAction.cs
        if (!item.action.IsValid(item)) return;

        item.action.Execute(item);
        GameManager.Instance.ShowNotice($"{StrUtiity.AttachJoSa(item.ItemName)} 완료 했습니다!");
        // 모루 강화를 완료 했습니다!
    }
}
