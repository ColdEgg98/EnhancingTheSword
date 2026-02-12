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
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(OpenPanel);

        SetItemButtons();

        X.onClick.AddListener(XButton);
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
        BuyItemPanel.SetActive(!BuyItemPanel.activeSelf);
    }

    public void XButton()
    {
        BuyItemPanel.SetActive(false);
    }
}
