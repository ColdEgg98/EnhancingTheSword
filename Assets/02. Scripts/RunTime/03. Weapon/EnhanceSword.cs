using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnhanceSword : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button enhanceButton;    // 강화 버튼
    [SerializeField] private Image targetImage;

    private int currentWeaponIndex;

    // Shader
    private Material _materialInstance;
    private int _flashID;

    // flag
    public bool isEnhancing;

    private void Awake()
    {
        currentWeaponIndex = GameManager.Instance.selectWeaponIndex.Value;

        // 프로퍼티 아이디 캐싱
        _flashID = Shader.PropertyToID("_FlashAmount");

        _materialInstance = targetImage.material;

        isEnhancing = false;
    }

    void Start()
    {
        // 버튼 이벤트 설정
        if (enhanceButton == null) enhanceButton = GetComponentInChildren<Button>();
        enhanceButton.onClick.AddListener(() =>
        {
            if (!isEnhancing)
                RunEnhancing().Forget();
            else
                Debug.Log("❌ 현재 강화중입니다.");
        });

        // 무기 정보 구독
        GameManager.Instance.selectWeaponIndex
            .Subscribe(index => currentWeaponIndex = index)
            .AddTo(this);
    }

    public async UniTask RunEnhancing()
    {
        // Flag On
        isEnhancing = true;

        // Sound
        GameManager.Instance.soundManager.PlaySFX("AnvilHit");

        Weapon currentWeapon = GameManager.Instance.currentWeapon.Value;

        // 소모 재화 계산
        long price = currentWeapon.enhancingPrice;
        if (GameManager.Instance.isFocusOn.Value) price += (long)(currentWeapon.enhancingPrice * 0.1f);

        // 강화 유효 판단
        if (IsValid(currentWeapon, price) == false)
            return;

        // 재화 소모
        GameManager.Instance.gold.Value -= price;
        Debug.Log($"{StrUtiity.ToWonFormat(price)} 만큼 재화 소모");

        // 강화 시도
        bool result = CheckSuccess(currentWeapon.probability);

        // 업적 체크
        GameManager.Instance.currentData.enhanceCount++;
        GameManager.Instance.achievementManager.CheckAchievement(ConditionType.ShotEnhance, GameManager.Instance.currentData.enhanceCount);

        // 애니메이션
        await EnhanceAnimation();

        // 강화 결과 처리
        if (result)
            EnhancingSuccessed(currentWeapon).Forget();
        else
            EnhancingFailed(currentWeapon);

        // 저장
        GameManager.Instance.saveDataManager.StartSave();
    }

    private bool IsValid(Weapon weapon, long price)
    {
        if (GameManager.Instance.currentWeapon.Value == null)
        {
            GameManager.Instance.uiManager.UIFactory.ShowNotice("선택된 무기가 없습니다", Color.white);
            return false;
        }

        // 레벨 상한 체크
        if (weapon.index >= 20)
        {
            GameManager.Instance.uiManager.UIFactory.ShowNotice("이미 최대 레벨에 도달했습니다", Color.white);
            return false;
        }

        // 재화 및 요구 아이템 체크
        if (GameManager.Instance.gold.Value < price)
        {
            GameManager.Instance.uiManager.UIFactory.ShowNotice("골드가 부족합니다", Color.white);
            return false;
        }

        return true;
    }

    private bool CheckSuccess(float p)
    {
        float bonus = GameManager.Instance.currentData.chanceBonus;
        if (GameManager.Instance.isFocusOn.Value)
            bonus = GameManager.Instance.currentData.chanceBonus + p * 0.1f;
        Debug.Log($"[EnhanceSword] chanceBonus : {bonus}로 적용됨.");
        float percent = p + bonus;
        return Random.value * 100 <= percent;
    }

    private async UniTask EnhanceAnimation()
    {
        _materialInstance.DOKill();
        _materialInstance.SetFloat(_flashID, 0);

        await _materialInstance.DOFloat(1f, _flashID, 0.75f).AsyncWaitForCompletion();
    }

    private async UniTask EnhancingSuccessed(Weapon currentWeapon)
    {
        // 1. 다음 단계 무기 데이터 가져오기
        int nextIndex = currentWeapon.index + 1;
        if (!GameManager.Instance.allOfWeaponDictionary.ContainsKey(nextIndex)) return;

        Weapon newWeapon = GameManager.Instance.allOfWeaponDictionary[nextIndex];

        // 2. 실제 데이터(GameManager) 갱신
        GameManager.Instance.currentData.myWeapons[currentWeaponIndex] = newWeapon;

        // 3. 무기 표시 변경
        GameManager.Instance.currentWeapon.Value = newWeapon;

        // Sound
        GameManager.Instance.soundManager.PlaySFX("WellDone");

        // SetFlag
        isEnhancing = false;

        // 10 레벨 업적 확인
        if (newWeapon.index == 10)
            GameManager.Instance.achievementManager.CheckAchievement(ConditionType.WeaponLevel, 10);

        // 5. 애니메이션
        _materialInstance.DOKill();
        await _materialInstance.DOFloat(0f, _flashID, 4f).AsyncWaitForCompletion();

        Debug.Log($"강화 성공: {newWeapon.name}");
    }

    private void EnhancingFailed(Weapon currentWeapon)
    {
        Debug.Log($"{currentWeapon.name} 파괴됨.");

        _materialInstance.SetFloat( _flashID, 0);      

        // 실제 데이터(GameManager)에서 삭제
        var myWeapons = GameManager.Instance.currentData.myWeapons;

        // 리스트에서 제거
        myWeapons.RemoveAt(currentWeaponIndex);

        // UI 갱신 & 0번에서 깨지고, 인벤에서 0번 누르면 반응할 수 있도록
        GameManager.Instance.selectWeaponIndex.Value = -1;

        // SetFlag
        isEnhancing = false;

        // 효과음
        GameManager.Instance.soundManager.PlaySFX("Break");

        // 업적 체크
        GameManager.Instance.currentData.failCount++;
        GameManager.Instance.achievementManager.CheckAchievement(ConditionType.FailEnhance, GameManager.Instance.currentData.failCount);
    }
}
