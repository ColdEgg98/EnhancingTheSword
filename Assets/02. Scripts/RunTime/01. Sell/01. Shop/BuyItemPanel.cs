using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyItemPanel : MonoBehaviour
{
    [SerializeField] private RectTransform ui;

    [Header("Icon")]
    [SerializeField] private Image itemIcon;

    [Header("TMP")]
    [SerializeField] private TextMeshProUGUI tmpTitle;
    [SerializeField] private TextMeshProUGUI tmpDescription;
    [SerializeField] private TextMeshProUGUI tmpPrice;

    [Header("Button")]
    [SerializeField] private Button btnYes;
    [SerializeField] private Button btnNo;

    private MaterialItem item;

    private void Awake()
    {
        btnYes.onClick.AddListener(YesButtonBehavior);
        btnNo.onClick.AddListener(ExitPurchase);
    }

    public async UniTask SetUICondition(MaterialItem item)
    {
        this.item = item;
        itemIcon.color = Color.clear;
        TmpSetting();
        ui.gameObject.SetActive(true);
        GetComponent<Image>().raycastTarget = true;

        await GameManager.Instance.aAResourceManager.SetSpriteAsync($"{item.AddressID}", itemIcon);
        itemIcon.color = Color.white;
    }

    private void TmpSetting()
    {
        tmpTitle.SetText(item.ItemName);
        tmpDescription.SetText(item.Description);
        StrUtiity.SetTmpText(tmpPrice, item.action.GetItemPrice(item)); // GC 부담 낮춤
    }

    private void YesButtonBehavior()
    {
        if (TryPurchase())
            ExitPurchase();
    }

    private bool TryPurchase()
    {
        if (!item.action.IsValid(item)) return false;

        if (item.IsConsumable)
        {
            GameManager.Instance.gold.Value -= item.ItemPrice;
            GameManager.Instance.userDataManager.GetItem($"{item.AddressID}");
            Debug.Log($"아이템 획득 : {item.ItemName}");
            return true;
        }

        item.action.Execute(item);
        GameManager.Instance.ShowNotice($"{StrUtiity.AttachJoSa(item.ItemName)} 완료 했습니다!");
        Debug.Log($"업그레이드 완료 : {item.ItemName}");
        return true;
        // 모루 강화를 완료 했습니다!
    }

    private void ExitPurchase()
    {
        GetComponent<Image>().raycastTarget = false;
        ui.gameObject.SetActive(false);
    }
}
