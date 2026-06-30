using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class SceneHandler : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image targetImage; // 무기 이미지
    private Material _material;
    private int _flashID;
    [SerializeField] private TextMeshProUGUI weaponNameText; // 무기 이름
    [SerializeField] private TextMeshProUGUI probabilityText; // 강화 확률
    [SerializeField] private TextMeshProUGUI InfoText; // 무기 강화 & 판매 정보 표기
    [SerializeField] private TextMeshProUGUI TipText; // 좌상단에 튜토리얼
    [SerializeField] private GameObject TesterButton; // 테스트 헬퍼
    private Weapon weapon;
    private bool isFocus;

    private void Awake()
    {
        _flashID = Shader.PropertyToID("_FlashAmount");
        _material = targetImage.material;
        SetSubscribe();
    }

    private void Start()
    {
        GameManager.Instance.uiManager.TipTextAppend("강화 하기 (Space)");
        GameManager.Instance.uiManager.TipTextAppend("가방 열기 (E)");
        GameManager.Instance.uiManager.TipTextAppend("판매 (S)");
#if UNITY_EDITOR
        TesterButton.SetActive(true);
#endif
    }

    private void SetSubscribe()
    {
        GameManager.Instance.currentWeapon
            .Subscribe(weapon =>
            {
                if (weapon == null)
                {
                    GameObjectsSetActive(false);
                }
                else
                {
                    LoadSprite(weapon.AddressID).Forget();
                    weaponNameText.text = weapon.WeaponName;

                    GameObjectsSetActive(true);
                }

                // 무기 정보(강화 비용 등) 갱신
                this.weapon = weapon;
                UpdateWeaponInfo(weapon);
            })
            .AddTo(this);

        // 인벤토리 등 무기 변경시 스프라이트 변경
        GameManager.Instance.selectWeaponIndex
            .Subscribe(index =>
            {
                if (GameManager.Instance.selectWeaponIndex.Value <= -1)
                {
                    GameManager.Instance.currentWeapon.Value = null;
                    return;
                }
                // 강화 중 무기 변경 시 연출 초기화
                _material.DOKill();
                _material.SetFloat(_flashID, 0f);

                if (GameManager.Instance.currentData.myWeapons.Count > index)
                    GameManager.Instance.currentWeapon.Value = GameManager.Instance.currentData.myWeapons[index];
            })
            .AddTo(this);

        // focus 켰을 때 UI 갱신
        GameManager.Instance.isFocusOn
            .Subscribe(boolean =>
            {
                isFocus = boolean;
                UpdateWeaponInfo(weapon);
            })
            .AddTo(this);

        // 좌상단 팁 뜨는거 관리
        GameManager.Instance.uiManager.tipList
            .ObserveCountChanged()
            .Subscribe(_ =>
            {
                string stringFormat = string.Join("\n", GameManager.Instance.uiManager.tipList);
                TipText.text = stringFormat;
            })
            .AddTo(this);
    }

    private void GameObjectsSetActive(bool v)
    {
        targetImage.gameObject.SetActive(v);
        weaponNameText.gameObject.SetActive(v);
        InfoText.gameObject.SetActive(v);
        probabilityText.gameObject.SetActive(v);
    }

    public async UniTask LoadSprite(string id)
    {
        IViewable viewable = GameManager.Instance.currentWeapon.Value;
        await GameManager.Instance.aAResourceManager.SetSpriteAsync(viewable, targetImage);
    }

    public void UpdateWeaponInfo(Weapon weapon)
    {
        // 파괴
        if (GameManager.Instance.selectWeaponIndex.Value == -1)
        {
            GameManager.Instance.uiManager.UIFactory.ShowNotice("무기가 파괴되었습니다", Color.red);
            return;
        }

        // 판매
        if (GameManager.Instance.selectWeaponIndex.Value == -2)
            return;

        if (weapon.Index == 20)
        {
            probabilityText.text = "마지막 단계에 도달했습니다.";
            InfoText.text = $"무기 강화 금액\n\t-\n" +
                            $"필요 아이템\n\t-\n" +
                            $"무기 판매 가격\n\t{StrUtiity.ToWonFormat(weapon.WeaponPrice)}";
            return;
        }
        else if (weapon.Index > GameManager.Instance.currentData.shopData.anvilLevel * 4)
        {
            probabilityText.text = "모루 레벨이 부족합니다! 상점에서 모루를 강화하세요.";
            InfoText.text = $"무기 강화 금액\n\t-\n" +
                            $"필요 아이템\n\t-\n" +
                            $"무기 판매 가격\n\t{StrUtiity.ToWonFormat(weapon.WeaponPrice)}";
            return;
        }

        // 집중 강화로 인한 표기 변경
        float Bonus = GameManager.Instance.currentData.chanceBonus;
        Bonus = (isFocus) ? Bonus + weapon.Probability * 0.1f : Bonus;

        long enhancingPrice = weapon.EnhancingPrice;
        long FocusPrice = weapon.EnhancingPrice / 10;
        enhancingPrice = (isFocus) ? weapon.EnhancingPrice + FocusPrice : enhancingPrice;

        string colorCode = (isFocus) ? "<color=#FF0000>" : "<color=#FFD700>";

        probabilityText.text = $"강화 확률 : <color=#FF0000>{weapon.Probability}%</color>";
        if (Bonus != 0f)
            probabilityText.text += $" + {colorCode}({Bonus}%)</color>";

        string tempFormat = $"무기 강화 금액\n\t{StrUtiity.ToWonFormat(enhancingPrice, colorCode)}\n" +
                            $"필요 아이템\n\t{ListToString(weapon.NeedItems)}\n" +
                            $"무기 판매 가격\n\t{StrUtiity.ToWonFormat(weapon.WeaponPrice)}";
        InfoText.text = tempFormat;
    }

    private string ListToString(List<int> list)
    {
        string str = (list == null || list.Count == 0) ? "-" : string.Join(",", list);
        return str;
    }
}
