using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyItemButtonBehavior : MonoBehaviour
{
    private Button thisButton;
    [Header("Price Text")]
    [SerializeField] private List<TextMeshProUGUI> pricetexts;

    [Header("Panel")]
    [SerializeField] private GameObject BuyItemPanel;

    [Header("GetComponentButton Target")]
    [SerializeField] private Transform targetTansform;
    private List<Button> buttons;

    [Header("X Button")]
    [SerializeField] private Button X;

    private void Awake()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(OpenPanel);

        buttons = targetTansform.GetComponentsInChildren<Button>().ToList();

        List<string> itemNames = new List<string>
        {
            "LowGrade AD",
            "MidGrade AD",
            "HighGrade AD",
            "ProUp"
        };
        List<long> prices = new List<long>
        {
           7000000,
           14000000,
           700000000,
           420000000
        };

        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;
            buttons[index].onClick.AddListener(() => GetItem(itemNames[index], prices[index]));
            pricetexts[index].text = $"{StrUtiity.ToWonFormat(prices[index])}";
        }

        X.onClick.AddListener(XButton);
    }

    public void OpenPanel()
    {
        BuyItemPanel.SetActive(!BuyItemPanel.activeSelf);
    }

    public void GetItem(string itemName, long price)
    {
        GameManager.Instance.userDataManager.BuyItem(itemName, price);
    }

    public void XButton()
    {
        BuyItemPanel.SetActive(false);
    }
}
