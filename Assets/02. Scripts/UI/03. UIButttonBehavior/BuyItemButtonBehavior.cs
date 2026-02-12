using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BuyItemButtonBehavior : MonoBehaviour
{
    private Button thisButton;

    [Header("Panel")]
    [SerializeField] private GameObject BuyItemPanel;

    [Header("Transform Target")]
    [SerializeField] private Transform GridTransform;

    [Header("Prefab")]
    [SerializeField] private GameObject ItemBtn;

    [Header("X Button")]
    [SerializeField] private Button X;

    private void Awake()
    {
        // 아이템 구매 버튼
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(OpenPanel);

        // 상점 UI 버튼 세팅
        SetItemButtons();

        // X버튼 바인딩
        X.onClick.AddListener(XButton);

        // TipText설정
        GameManager.Instance.TipTextAppend("아이템 상점 (I)");
    }

    private void SetItemButtons()
    {
        // 소모품 갯수만큼 버튼 생성 & 버튼 세팅
        foreach (MaterialItem m in GameManager.Instance.allOfItemsDictionary.Values.OfType<MaterialItem>())
        {
            if (m.IsConsumable)
            {
                GameObject instance = Instantiate(ItemBtn, GridTransform);
                BuyItemButton buyItemBtn = instance.GetComponent<BuyItemButton>();
                buyItemBtn.SetBuyItemButton(m.ItemName, m.ItemPrice, m.AddressID);
            }
        }
    }

    public void OpenPanel()
    {
        GameManager.Instance.ModifyTipText("아이템 상점 (I)");
        GameManager.Instance.ModifyTipText("상점 닫기 (I)");

        BuyItemPanel.SetActive(!BuyItemPanel.activeSelf);
    }

    public void XButton()
    {
        GameManager.Instance.ModifyTipText("아이템 상점 (I)");
        GameManager.Instance.ModifyTipText("상점 닫기 (I)");

        BuyItemPanel.SetActive(false);
    }
}
