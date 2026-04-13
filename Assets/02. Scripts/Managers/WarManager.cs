using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class WarManager : MonoBehaviour
{
    // ───────────────────────────────────────
    // 전황 수치
    // ───────────────────────────────────────

    [SerializeField] private FloatReactiveProperty _warGauge = new FloatReactiveProperty(50f);
    public IReadOnlyReactiveProperty<float> WarGaugeFloat => _warGauge;

    private const float MAX_GAUGE = 100f;
    private const float MIN_GAUGE = 0f;

    // ───────────────────────────────────────
    // 스테이지
    // ───────────────────────────────────────
    [SerializeField] private IntReactiveProperty _currentStage = new IntReactiveProperty(1);
    public IReadOnlyReactiveProperty<int> CurrentStage => _currentStage;

    private readonly Dictionary<int, float> _decayRatePerStage = new Dictionary<int, float>
    {
        { 1, 0.03f },
        { 2, 0.1f },
        { 3, 0.3f },
        { 4, 0.4f },
        { 5, 0.6f },
        { 6, 0.8f },
        { 7, 1f },
        { 8, 1.4f },
        { 9, 1.6f },
        { 10, 2f }
    };

    private const float NEXT_STAGE_THRESHOLD = 95f;
    private const float NEXT_STAGE_HOLD_TIME = 10f;
    private float _aboveThresholdTimer = 0f;

    // ───────────────────────────────────────
    // 출하 슬롯
    // ───────────────────────────────────────
    private int _maxSlot;
    private readonly ReactiveCollection<DeliverySlot> _slots = new ();

    // 사용중인 슬롯의 인덱스 기록
    private readonly List<int> _activatedSlotIds = new();

    // 외부에서 출하 중인 무기 인덱스 확인용
    public IReadOnlyReactiveCollection<DeliverySlot> Slots => _slots;
    public List<int> ActivatedSlotIds => _activatedSlotIds;

    // ───────────────────────────────────────
    // Awake
    // ───────────────────────────────────────
    void Awake()
    {
        Sub();
    }

    private void Sub()
    {
        // 사용자의 출하 슬롯 갯수가 변경될때마다 maxSlot 변경
        GameManager.Instance.shippingSlot
            .Subscribe(value => _maxSlot = value)
            .AddTo(this);
    }

    // ───────────────────────────────────────
    // Update
    // ───────────────────────────────────────
    private void Update()
    {
        float delta = Time.deltaTime;

        ApplyDecay(delta);
        UpdateSlots(delta);
        CheckStageClear(delta);
        CheckGameOver();
    }

    // ───────────────────────────────────────
    // 전황 자동 감소
    // ───────────────────────────────────────
    private void ApplyDecay(float delta)
    {
        float rate = _decayRatePerStage.TryGetValue(_currentStage.Value, out float r) ? r : 1f;
        _warGauge.Value = Mathf.Clamp(_warGauge.Value - rate * delta, MIN_GAUGE, MAX_GAUGE);
    }

    // ───────────────────────────────────────
    // 슬롯 업데이트
    // ───────────────────────────────────────
    private void UpdateSlots(float delta)
    {
        for (int i = _slots.Count - 1; i >= 0; i--)
        {
            var slot = _slots[i];

            // 1단계: 출하 대기 중
            if (!slot.IsDelivered)
            {
                slot.DeliveryTimeRemaining -= delta;
                if (slot.DeliveryTimeRemaining <= 0f)
                {
                    slot.IsDelivered = true;
                    Debug.Log($"{slot.WeaponName} 출하 완료! 영향력 적용 시작");
                }
                continue;
            }

            // 2단계: 도착 후 영향력 적용 (유지 시간 내내 누적)
            if (slot.InfluenceTimeRemaining > 0f)
            {
                float contribution = slot.TotalInfluence * delta;
                _warGauge.Value = Mathf.Clamp(_warGauge.Value + contribution, MIN_GAUGE, MAX_GAUGE);
                slot.InfluenceTimeRemaining -= delta;
            }
            else
            {
                // 영향력 유지 시간 종료
                Debug.Log($"{slot.WeaponName} 영향력 종료");
                _activatedSlotIds.Remove(slot.SlotIndex);
                _slots.RemoveAt(i);
            }
        }
    }

    // ───────────────────────────────────────
    // 무기 출하 무결성 검사
    // ───────────────────────────────────────
    public bool IsVaildDeliverWeapon(int weaponIndex, int slotIndex)
    {
        if (_activatedSlotIds.Contains(slotIndex))
        {
            // TODO : 사용중인 슬롯의 무기 제거하고 새로 낄건지 확인하는 UI

            GameManager.Instance.ShowNotice("이미 사용중인 슬롯입니다.");
            return false;
        }

        if (_slots.Count >= _maxSlot)
        {
            GameManager.Instance.ShowNotice("출하 슬롯이 가득 찼습니다.");
            return false;
        }

        List<Weapon> myWeapons = GameManager.Instance.GetMyWeapons();
        if (weaponIndex < 0 || weaponIndex >= myWeapons.Count)
        {
            GameManager.Instance.ShowNotice("유효하지 않은 무기 인덱스입니다.");
            return false;
        }
        return true;
    }

    // ───────────────────────────────────────
    // 무기 출하 등록 (GameManager에서 선택된 무기 받기)
    // ───────────────────────────────────────
    public void DeliverWeapon(int weaponIndex, int slotIndex)
    {
        List<Weapon> myWeapons = GameManager.Instance.GetMyWeapons();
        Weapon weapon = myWeapons[weaponIndex];
        _slots.Add(new DeliverySlot(weaponIndex, slotIndex, weapon));
        _activatedSlotIds.Add(slotIndex);
        Debug.Log($"{weapon.WeaponName} 출하 시작 / 출하 시간: {weapon.DeliveryTime}s / 영향력: {weapon.WarInfluence} / 유지: {weapon.InfluenceDuration}s");
        UsedWeaponRemove(weaponIndex);
    }

    public bool TryDeliverWeapon(int weaponIndex, int slotIndex)
    {
        // 무결성 검사
        if (!IsVaildDeliverWeapon(weaponIndex, slotIndex)) return false;

        // 무기 배달 및 삭제
        DeliverWeapon(weaponIndex, slotIndex);
        
        return true;
    }

    // 출하된 무기 삭제
    private void UsedWeaponRemove(int index) => GameManager.Instance.currentData.myWeapons.RemoveAt(index);

    // 출하 중인지 여부 확인 (인벤토리 UI 잠금용)
    public bool IsDelivering(int weaponIndex)
    {
        return _slots.Any(s => s.WeaponIndex == weaponIndex);
    }

    // ───────────────────────────────────────
    // 스테이지 클리어 / 실패
    // ───────────────────────────────────────
    private void CheckStageClear(float delta)
    {
        // 전황 수치 95이상으로 5초 유지시
        if (_warGauge.Value >= NEXT_STAGE_THRESHOLD)
        {
            _aboveThresholdTimer += delta;
            if (_aboveThresholdTimer >= NEXT_STAGE_HOLD_TIME)
            {
                _aboveThresholdTimer = 0f;
                GoNextStage();
            }
        }
        else
        {
            _aboveThresholdTimer = 0f;
        }
    }

    private void CheckGameOver()
    {
        if (_warGauge.Value <= MIN_GAUGE)
            GoPreviousStage();
    }

    private void GoNextStage()
    {
        // TODO: 게임 클리어 처리

        _currentStage.Value++;
        ResetGauge();
        GameManager.Instance.ShowNotice($"전투 승리!\n{_currentStage}스테이지로 진입합니다.");
        Debug.Log($"다음 스테이지 진입: {_currentStage.Value}");
    }

    private void GoPreviousStage()
    {
        if (_currentStage.Value <= 1)
        {
            Debug.Log("게임 오버");
            // TODO: 게임 오버 처리
            return;
        }
        _currentStage.Value--;
        ResetGauge();
        GameManager.Instance.ShowNotice($"전투에서 패배했습니다..\n{_currentStage}스테이지로 후퇴합니다.");
        Debug.Log($"이전 스테이지로 밀려남: {_currentStage.Value}");
    }

    private void ResetGauge()
    {
        _warGauge.Value = 50f;
    }

    // ───────────────────────────────────────
    // 슬롯 해금 (상점에서 호출)
    // ───────────────────────────────────────
    public void UnlockSlot()
    {
        if (_maxSlot < 5)
            _maxSlot++;
    }
}

// ───────────────────────────────────────
// 출하 슬롯 데이터
// ───────────────────────────────────────
public class DeliverySlot
{
    public int WeaponIndex;
    public int SlotIndex;
    public string WeaponName;

    public float DeliveryTimeRemaining;  // 출하 대기 시간
    public bool IsDelivered;            // 도착 완료 여부

    public float TotalInfluence;         // 총 영향력
    public float TotalDuration;          // 총 유지 시간
    public float InfluenceTimeRemaining; // 남은 유지 시간

    public DeliverySlot(int index, int slotIndex, Weapon weapon)
    {
        WeaponIndex = index;
        SlotIndex = slotIndex;
        WeaponName = weapon.WeaponName;
        DeliveryTimeRemaining = weapon.DeliveryTime;
        IsDelivered = false;
        TotalInfluence = weapon.WarInfluence;
        TotalDuration = weapon.InfluenceDuration;
        InfluenceTimeRemaining = weapon.InfluenceDuration;
    }
}