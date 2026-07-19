using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SellWeapon : MonoBehaviour
{
    [SerializeField] private Button quickSellBtn;
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;
    [SerializeField] private TextMeshProUGUI sellingWeaponsTxt;
    [SerializeField] private TextMeshProUGUI PriceTxt;
    [SerializeField] private GameObject Panel;
    public InventoryView inventoryData;
    
    ReactiveCollection<Weapon> myWeapons;
    List<MaterialItem> myMaterials;
    private string currentWeaponName;
    private long price;

    void Awake()
    {
        inventoryData = FindAnyObjectByType<InventoryView>();

        quickSellBtn.onClick.AddListener(() => SetPanel());
        yesBtn.onClick.AddListener(YesBtnBehavior);
        noBtn.onClick.AddListener(() => Panel.SetActive(false));
    }

    public void SetPanel()
    {
        // 변수 세팅
        Setting();

        // 유효성 체크
        if(!IsValid())
            return;

        // UI에 텍스트 적용
        sellingWeaponsTxt.text = currentWeaponName;
        PriceTxt.text = StrUtiity.ToWonFormat(price);

        // 효과음
        GameManager.Instance.soundManager.PlaySFX("Click");

        // 패널 등장
        Panel.SetActive(true);
    }

    private void Setting()
    {
        if (GameManager.Instance.currentWeapon.Value != null)
        {
            currentWeaponName = GameManager.Instance.currentWeapon.Value.WeaponName;
            price = GameManager.Instance.currentWeapon.Value.WeaponPrice;
        }
        else
        {
            currentWeaponName = string.Empty;
            price = 0;
        }

        myWeapons = GameManager.Instance.currentData.myWeapons;
        myMaterials = GameManager.Instance.currentData.materials;
    }

    private bool IsValid()
    {
        if (GameManager.Instance.selectWeaponIndex.Value <= -1)
        {
            GameManager.Instance.uiManager.UIFactory.ShowNotice("판매할 무기가 없습니다", Color.white);
            return false;
        }

        return true;
    }

    private void YesBtnBehavior()
    {
        // 효과음
        GameManager.Instance.soundManager.PlaySFX("Click");

        // 패널 끄기
        Panel.SetActive(false);

        // 인벤 정리
        EliminateProcess().Forget(); // UniTaskVoid 호출 시 Forget() 권장
        inventoryData.HideInventory();

        // 골드 지급
        GameManager.Instance.userDataManager.GetGold(price);
    }

    private async UniTaskVoid EliminateProcess()
    {
        // 현재 선택된 무기 하나만 삭제
        myWeapons.RemoveAt(GameManager.Instance.selectWeaponIndex.Value);

        // 판매 후 인덱스 변경으로 UI 표기 변경
        GameManager.Instance.selectWeaponIndex.Value = -2;
        await UniTask.Yield();
    }
}