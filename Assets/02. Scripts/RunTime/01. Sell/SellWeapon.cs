using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellWeapon : MonoBehaviour
{
    public Dictionary <int, Weapon> weaponsForSell;
    [SerializeField] private Button sellBtn;
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;
    [SerializeField] private TextMeshProUGUI sellingWeaponsTxt;
    [SerializeField] private TextMeshProUGUI PriceTxt;
    [SerializeField] private GameObject Panel;
    
    List<int> indexList;
    List<Weapon> myWeapons;
    private string currentWeaponName;
    private long price;
    private bool IsDictHasData;

    void Awake()
    {
        sellBtn.onClick.AddListener(() => SetPanel());
        yesBtn.onClick.AddListener(YesBtnBehavior);
        noBtn.onClick.AddListener(() => Panel.SetActive(false));
        weaponsForSell = new();
    }

    public void SetPanel(Dictionary<int, Weapon> pairs = null)
    {
        // 유효성 체크
        if(!IsValid())
            return;

        // 변수 세팅
        Setting(pairs);

        // UI에 텍스트 적용
        sellingWeaponsTxt.text = (weaponsForSell.Count > 0) ? GetSalesString() : currentWeaponName;
        PriceTxt.text = StrUtiity.ToWonFormat(price);

        Panel.SetActive(true);
    }

    private bool IsValid()
    {
        if (GameManager.Instance.selectWeaponIndex.Value <= -1 && IsDictHasData == false)
        {
            GameManager.Instance.uiManager.UIFactory.ShowNotice("판매할 무기가 없습니다", Color.white);
            return false;
        }

        if (weaponsForSell.Count == 0 && IsDictHasData == true)
        {
            GameManager.Instance.uiManager.UIFactory.ShowNotice("판매할 무기를 선택해주세요", Color.white);
            return false;
        }
        return true;
    }

    private void Setting(Dictionary<int, Weapon> pairs = null)
    {
        currentWeaponName = GameManager.Instance.currentWeapon.Value.name;
        myWeapons = GameManager.Instance.currentData.myWeapons;
        // dictSetting
        IsDictHasData = false;
        indexList = new();
        price = (weaponsForSell.Count > 0) ? 0 : GameManager.Instance.currentWeapon.Value.price;
    }

    private string GetSalesString()
    {
        StringBuilder sb = new();
        sb.Append("판매 목록 : ");
        foreach (var w in weaponsForSell)
        {
            sb.AppendJoin(", ", w.Value.name);
            price += w.Value.price;
        }
        return sb.ToString();
    }

    private void YesBtnBehavior()
    {
        Panel.SetActive(false);
        EliminateProcess();
        GameManager.Instance.userDataManager.GetGold(price);
    }

    private void EliminateProcess()
    {
        if (IsDictHasData)
        {
            foreach (var w in weaponsForSell)
            {
                myWeapons.RemoveAt(w.Key);
            }
        }
        else
        {
            myWeapons.RemoveAt(GameManager.Instance.selectWeaponIndex.Value);
        }
        GameManager.Instance.selectWeaponIndex.Value = -2;
    }
}
