using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellWeapon : MonoBehaviour
{
    public IDictionary <int, IViewable> IViewableForSell;
    [SerializeField] private Button quickSellBtn;
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;
    [SerializeField] private TextMeshProUGUI sellingWeaponsTxt;
    [SerializeField] private TextMeshProUGUI PriceTxt;
    [SerializeField] private GameObject Panel;
    public InventoryButtonBehavior inventoryData;
    
    List<Weapon> myWeapons;
    List<MaterialItem> myMaterials;
    private string currentWeaponName;
    private long price;
    public bool isDictHasData;

    void Awake()
    {
        inventoryData = FindAnyObjectByType<InventoryButtonBehavior>();

        quickSellBtn.onClick.AddListener(() => SetPanel());
        inventoryData.multiSellButton.onClick.AddListener(() => SetPanel(inventoryData.IViewableForSell));
        yesBtn.onClick.AddListener(YesBtnBehavior);
        noBtn.onClick.AddListener(() => Panel.SetActive(false));
    }

    public void SetPanel(IDictionary<int, IViewable> pairs = null)
    {
        // 변수 세팅
        Setting(pairs);

        // 유효성 체크
        if(!IsValid())
            return;

        // UI에 텍스트 적용
        sellingWeaponsTxt.text = (isDictHasData) ? GetSalesString() : currentWeaponName;
        PriceTxt.text = StrUtiity.ToWonFormat(price);

        // 효과음
        GameManager.Instance.soundManager.PlaySFX("Click");

        // 패널 등장
        Panel.SetActive(true);
    }

    private void Setting(IDictionary<int, IViewable> pairs = null)
    {
        if (GameManager.Instance.currentWeapon.Value != null)
            currentWeaponName = GameManager.Instance.currentWeapon.Value.WeaponName;
        myWeapons = GameManager.Instance.currentData.myWeapons;
        myMaterials = GameManager.Instance.currentData.materials;
        IViewableForSell = pairs;
        isDictHasData = pairs != null && pairs.Count > 0;
        // 중복 판매에 따라 값 세팅
        if (isDictHasData)
            price = 0;
        else if (GameManager.Instance.currentWeapon.Value != null)
            price = GameManager.Instance.currentWeapon.Value.WeaponPrice;
    }

    private bool IsValid()
    {
        if (GameManager.Instance.selectWeaponIndex.Value <= -1 && isDictHasData == false)
        {
            GameManager.Instance.uiManager.UIFactory.ShowNotice("판매할 무기가 없습니다", Color.white);
            return false;
        }

        if (isDictHasData && (IViewableForSell == null || IViewableForSell.Count == 0))
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
        sb.AppendJoin(", ", IViewableForSell.Values.Select(w => w.IViewableName));
        foreach (var w in IViewableForSell)
        {
            price += w.Value.IViewablePrice;
        }
        return sb.ToString();
    }

    private void YesBtnBehavior()
    {
        // 효과음
        GameManager.Instance.soundManager.PlaySFX("Click");

        // 패널 끄기
        Panel.SetActive(false);

        // 인벤 정리
        EliminateProcess();
        inventoryData.OnClickXButton();

        // 골드 지급
        GameManager.Instance.userDataManager.GetGold(price);

        if (IViewableForSell != null) IViewableForSell.Clear();
    }

    private async UniTaskVoid EliminateProcess()
    {
        if (isDictHasData)
        {
            // 내림차순 정렬
            List<int> DeathNote = IViewableForSell.Keys.OrderByDescending(k => k).ToList();

            if (inventoryData.isWeaponCategory)
            {
                foreach (int index in DeathNote)
                {
                    myWeapons.RemoveAt(index);
                }
            }

            else
            {
                foreach (int index in DeathNote)
                {
                    myMaterials.RemoveAt(index);
                }
            }
        }
        else
        {
            myWeapons.RemoveAt(GameManager.Instance.selectWeaponIndex.Value);
        }

        // 판매 후 인덱스 변경으로 UI 표기 변경
        int tempIndex = GameManager.Instance.selectWeaponIndex.Value;
        GameManager.Instance.selectWeaponIndex.Value = -2;
        await UniTask.Yield();
        GameManager.Instance.selectWeaponIndex.Value = tempIndex;
    }
}
