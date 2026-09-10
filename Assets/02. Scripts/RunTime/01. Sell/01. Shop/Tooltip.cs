using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

// 상점 아이템의 아이템 정보를 기억합니다.
public class Tooltip : MonoBehaviour
{
    private BuyItemPanel purchaseUI;
    private Button shopButton;

    [SerializeField] private string itemId;
    private MaterialItem item;

    void Awake()
    {
        purchaseUI = FindAnyObjectByType<BuyItemPanel>();
        shopButton = GetComponent<Button>();
        shopButton.onClick.AddListener(ButtonSub);

        item = (MaterialItem)GameManager.Instance.allOfItemsDictionary[itemId];
    }

    private void ButtonSub()
    {
        purchaseUI.SetUICondition(item).Forget();
    }
}
