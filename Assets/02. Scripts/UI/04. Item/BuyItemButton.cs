using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyItemButton : MonoBehaviour
{
    private Button _btn;
    [SerializeField] private TextMeshProUGUI NameText;
    [SerializeField] private TextMeshProUGUI PriceText;
    private string itemID;
    private long price;

    void Awake()
    {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(GetItem);
    }

    public void SetBuyItemButton(string name, long price, string itemID)
    {
        NameText.text = name;
        PriceText.text = StrUtiity.ToWonFormat(price);
        this.price = price;
        this.itemID = itemID;
    }

    private void GetItem()
    {
        GameManager.Instance.userDataManager.BuyItem(itemID, price);
    }
}
