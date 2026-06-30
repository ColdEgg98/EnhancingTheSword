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

        // 아이템 정보 취득
        item = (MaterialItem)GameManager.Instance.allOfItemsDictionary[itemID];
    }
}
