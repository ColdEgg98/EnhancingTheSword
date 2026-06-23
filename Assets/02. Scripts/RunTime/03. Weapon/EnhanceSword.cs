using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnhanceSword : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button enhanceButton;    // 강화 버튼
    [SerializeField] private Image targetImage;

    [Header("Minigame")]
    [SerializeField] private StrikePoint strikePoint;
    [Range(0f, 100f)]
    [SerializeField] private float miniGameChance;

    private int currentWeaponIndex;

    // Shader
    private Material _materialInstance;
    private int _flashID;

    // flag
    public bool isEnhancing;

    private EnhancingEffect EffectHandler;

    private void Awake()
    {
        currentWeaponIndex = GameManager.Instance.selectWeaponIndex.Value;

        // 프로퍼티 아이디 캐싱
        _flashID = Shader.PropertyToID("_FlashAmount");

        _materialInstance = targetImage.material;

        isEnhancing = false;

        EffectHandler = GetComponent<EnhancingEffect>();
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

        // 강화 유효 판단
        if (IsValid(currentWeapon, out long price) == false)
        {
            isEnhancing = false;
            return;
        }

        // 재화 소모
        GameManager.Instance.gold.Value -= price;
        Debug.Log($"{StrUtiity.ToWonFormat(price)} 만큼 재화 소모");

        // 미니게임 발생 및 결과 저장
        MiniGameResult miniGameResult = await TryTriggerMiniGame();

        // 강화 시도
        bool result = CheckSuccess(currentWeapon.Probability, miniGameResult);

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

    private async Task<MiniGameResult> TryTriggerMiniGame()
    {
        bool triggered = Random.value * 100f <= miniGameChance;

        if (triggered && strikePoint != null)
            return await strikePoint.Play();

        return new MiniGameResult { isPlayed = false };
    }

    private bool IsValid(Weapon weapon, out long price)
    {
        price = 0;

        // 무기 유무 체크
        if (GameManager.Instance.currentWeapon.Value == null)
        {
            GameManager.Instance.uiManager.UIFactory.ShowNotice("선택된 무기가 없습니다", Color.white);
            return false;
        }

        // 레벨 상한 체크
        if (weapon.Index >= 20)
        {
            GameManager.Instance.uiManager.UIFactory.ShowNotice("이미 최대 레벨에 도달했습니다", Color.white);
            return false;
        }

        // 소모 재화 계산
        price = weapon.EnhancingPrice;
        if (GameManager.Instance.isFocusOn.Value) price += (long)(weapon.EnhancingPrice * 0.1f);

        // 재화 및 요구 아이템 체크
        if (GameManager.Instance.gold.Value < price)
        {
            GameManager.Instance.ShowNotice("골드가 부족합니다");
            return false;
        }

        // 모루 레벨 체크 (모루 렙 *4 까지)
        if (GameManager.Instance.currentData.shopData.anvilLevel * 4 <= weapon.Index)
        {
            GameManager.Instance.ShowNotice("모루 레벨이 부족합니다.");
            return false;
        }

        return true;
    }

    private bool CheckSuccess(float p, MiniGameResult result)
    {
        float bonus = GameManager.Instance.currentData.chanceBonus;

        // 집중 강화 확률 적용 (10% 상승)
        if (GameManager.Instance.isFocusOn.Value)
            bonus = GameManager.Instance.currentData.chanceBonus + p * 0.1f;

        // 미니게임 결과 확률 (최대 15%, miss시 -5%)
        float miniGameBonus = result.GetBonusChance();
        Debug.Log($"미니게임 확률 적용 : {miniGameBonus}");
        p += p * miniGameBonus;

        Debug.Log($"[EnhanceSword] chanceBonus : {bonus}로 적용됨.");
        float percent = p + bonus;
        return Random.value * 100 <= percent;
    }

    private async UniTask EnhanceAnimation()
    {
        _materialInstance.DOKill();
        _materialInstance.SetFloat(_flashID, 0);

        // Paticle
        EffectHandler.StartEffect();

        await _materialInstance.DOFloat(1f, _flashID, 1f).AsyncWaitForCompletion();
    }

    private async UniTask EnhancingSuccessed(Weapon currentWeapon)
    {
        // 1. 다음 단계 무기 데이터 가져오기
        int nextIndex = currentWeapon.Index + 1;
        if (!GameManager.Instance.allOfWeaponDictionary.ContainsKey(nextIndex)) return;

        Weapon newWeapon = GameManager.Instance.allOfWeaponDictionary[nextIndex];

        // 2. 실제 데이터(GameManager) 갱신
        GameManager.Instance.currentData.myWeapons[currentWeaponIndex] = newWeapon;

        // 3. 무기 표시 변경
        GameManager.Instance.currentWeapon.Value = newWeapon;

        // Sound
        GameManager.Instance.soundManager.PlaySFX("WellDone");

        // Particle
        EffectHandler.EndEffect();

        // SetFlag
        isEnhancing = false;

        // 10 레벨 업적 확인
        if (newWeapon.Index == 10)
            GameManager.Instance.achievementManager.CheckAchievement(ConditionType.WeaponLevel, 10);

        // 5. 애니메이션
        _materialInstance.DOKill();
        await _materialInstance.DOFloat(0f, _flashID, 4f).AsyncWaitForCompletion();

        Debug.Log($"강화 성공: {newWeapon.WeaponName}");
    }

    private void EnhancingFailed(Weapon currentWeapon)
    {
        if (currentWeapon.IsAntiDestruction)
        {
            _materialInstance.SetFloat(_flashID, 0);
            currentWeapon.IsAntiDestruction = false;
            GameManager.Instance.soundManager.PlaySFX("Success");
            GameManager.Instance.ShowNotice("파괴 방지 물약으로 인해\n무기가 파괴되지 않았습니다.");
            isEnhancing = false;
            return;
        }

        Debug.Log($"{currentWeapon.WeaponName} 파괴됨.");

        // 진동
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR
        Handheld.Vibrate();
#endif

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
