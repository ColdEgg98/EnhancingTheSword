using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellWeapon : MonoBehaviour
{
    public IDictionary <int, Weapon> weaponsForSell;
    [SerializeField] private Button quickSellBtn;
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;
    [SerializeField] private TextMeshProUGUI sellingWeaponsTxt;
    [SerializeField] private TextMeshProUGUI PriceTxt;
    [SerializeField] private GameObject Panel;
    public InventoryButtonBehavior inventoryData;
    
    List<int> indexList;
    List<Weapon> myWeapons;
    private string currentWeaponName;
    private long price;
    private bool isDictHasData;

    void Awake()
    {
        inventoryData = FindAnyObjectByType<InventoryButtonBehavior>();

        quickSellBtn.onClick.AddListener(() => SetPanel());
        inventoryData.multiSellButton.onClick.AddListener(() => SetPanel(inventoryData.weaponsForSell));
        yesBtn.onClick.AddListener(YesBtnBehavior);
        noBtn.onClick.AddListener(() => Panel.SetActive(false));
    }

    public void SetPanel(IDictionary<int, Weapon> pairs = null)
    {
        // 변수 세팅
        Setting(pairs);

        // 유효성 체크
        if(!IsValid())
            return;

        // UI에 텍스트 적용
        sellingWeaponsTxt.text = (isDictHasData) ? GetSalesString() : currentWeaponName;
        PriceTxt.text = StrUtiity.ToWonFormat(price);

        Panel.SetActive(true);
    }

    private void Setting(IDictionary<int, Weapon> pairs = null)
    {
        if (GameManager.Instance.currentWeapon.Value != null)
            currentWeaponName = GameManager.Instance.currentWeapon.Value.name;
        myWeapons = GameManager.Instance.currentData.myWeapons;
        weaponsForSell = pairs;
        isDictHasData = pairs != null && pairs.Count > 0;
        indexList = new();
        price = (isDictHasData) ? 0 : GameManager.Instance.currentWeapon.Value.price;
    }

    private bool IsValid()
    {
        if (GameManager.Instance.selectWeaponIndex.Value <= -1 && isDictHasData == false)
        {
            GameManager.Instance.uiManager.UIFactory.ShowNotice("판매할 무기가 없습니다", Color.white);
            return false;
        }

        if (isDictHasData && (weaponsForSell == null || weaponsForSell.Count == 0))
        {
            GameManager.Instance.uiManager.UIFactory.ShowNotice("판매할 무기를 선택해주세요", Color.white);
            return false;
        }
        return true;
    }

    private string GetSalesString()
    {
        StringBuilder sb = new();
        sb.Append("판매 목록 : ");
        sb.AppendJoin(", ", weaponsForSell.Values.Select(w => w.name));
        foreach (var w in weaponsForSell)
        {
            price += w.Value.price;
        }
        return sb.ToString();
    }

    private void YesBtnBehavior()
    {
        Panel.SetActive(false);
        EliminateProcess();
        inventoryData.OnClickXButton();
        GameManager.Instance.userDataManager.GetGold(price);

        if (weaponsForSell != null) weaponsForSell.Clear();
    }

    private void EliminateProcess()
    {
        if (isDictHasData)
        {
            // 내림차순 정렬
            List<int> DeathNote = weaponsForSell.Keys.OrderByDescending(k => k).ToList();

            foreach (int index in DeathNote) {
                myWeapons.RemoveAt(index);
            }
        }
        else
        {
            myWeapons.RemoveAt(GameManager.Instance.selectWeaponIndex.Value);
        }
        GameManager.Instance.selectWeaponIndex.Value = -2;
    }
}
