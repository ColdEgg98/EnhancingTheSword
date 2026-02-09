using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BuyItemButtonBehavior : MonoBehaviour
{
    private Button thisButton;
    private List<string> itemNames = new();

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
        itemNames = new List<string>
        {
            "LowGrade AD",
            "MidGrade AD",
            "HighGrade AD",
            "ProUp"
        };

        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;
            buttons[index].onClick.AddListener(() => GetItem(itemNames[index]));
        }

        X.onClick.AddListener(XButton);
    }

    public void OpenPanel()
    {
        BuyItemPanel.SetActive(!BuyItemPanel.activeSelf);
    }

    public void GetItem(string itemName)
    {
        GameManager.Instance.userDataManager.GetItem(itemName);
    }

    public void XButton()
    {
        BuyItemPanel.SetActive(false);
    }
}
