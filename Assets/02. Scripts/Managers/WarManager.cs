using System.Collections.Generic;
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
        { 1, 0.3f },
        { 2, 1.0f },
        { 3, 1.5f },
    };

    private const float NEXT_STAGE_THRESHOLD = 95f;
    private const float NEXT_STAGE_HOLD_TIME = 30f;
    private float _aboveThresholdTimer = 0f;

    // ───────────────────────────────────────
    // 출하 슬롯
    // ───────────────────────────────────────
    private int _maxSlot = GameManager.Instance.currentData.shippingSlot;
    private readonly List<DeliverySlot> _slots = new List<DeliverySlot>();

    // 외부에서 출하 중인 무기 인덱스 확인용
    public IReadOnlyList<DeliverySlot> Slots => _slots;

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

            // 2단계: 도착 후 영향력 적용 (B타입: 유지 시간 내내 부드럽게 누적)
            if (slot.InfluenceTimeRemaining > 0f)
            {
                float contribution = (slot.TotalInfluence / slot.TotalDuration) * delta;
                _warGauge.Value = Mathf.Clamp(_warGauge.Value + contribution, MIN_GAUGE, MAX_GAUGE);
                slot.InfluenceTimeRemaining -= delta;
            }
            else
            {
                // 영향력 유지 시간 종료
                Debug.Log($"{slot.WeaponName} 영향력 종료");
                _slots.RemoveAt(i);
            }
        }
    }

    // ───────────────────────────────────────
    // 무기 출하 등록 (GameManager에서 선택된 무기 받기)
    // ───────────────────────────────────────
    public bool TryDeliverWeapon(int weaponIndex)
    {
        if (_slots.Count >= _maxSlot)
        {
            Debug.LogWarning("출하 슬롯이 가득 찼습니다.");
            return false;
        }

        // 이미 출하 중인 무기인지 체크
        if (_slots.Exists(s => s.WeaponIndex == weaponIndex))
        {
            Debug.LogWarning("이미 출하 중인 무기입니다.");
            return false;
        }

        List<Weapon> myWeapons = GameManager.Instance.GetMyWeapons();
        if (weaponIndex < 0 || weaponIndex >= myWeapons.Count)
        {
            Debug.LogWarning("유효하지 않은 무기 인덱스입니다.");
            return false;
        }

        Weapon weapon = myWeapons[weaponIndex];
        _slots.Add(new DeliverySlot(weaponIndex, weapon));
        Debug.Log($"{weapon.WeaponName} 출하 시작 / 출하 시간: {weapon.DeliveryTime}s / 영향력: {weapon.WarInfluence} / 유지: {weapon.InfluenceDuration}s");
        return true;
    }

    // 출하 중인지 여부 확인 (인벤토리 UI 잠금용)
    public bool IsDelivering(int weaponIndex)
    {
        return _slots.Exists(s => s.WeaponIndex == weaponIndex);
    }

    // ───────────────────────────────────────
    // 스테이지 클리어 / 실패
    // ───────────────────────────────────────
    private void CheckStageClear(float delta)
    {
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
        _currentStage.Value++;
        ResetGauge();
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
        Debug.Log($"이전 스테이지로 밀려남: {_currentStage.Value}");
    }

    private void ResetGauge()
    {
        _warGauge.Value = 50f;
        _slots.Clear();
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
    public string WeaponName;

    public float DeliveryTimeRemaining;  // 출하 대기 시간
    public bool IsDelivered;            // 도착 완료 여부

    public float TotalInfluence;         // 총 영향력
    public float TotalDuration;          // 총 유지 시간
    public float InfluenceTimeRemaining; // 남은 유지 시간

    public DeliverySlot(int index, Weapon weapon)
    {
        WeaponIndex = index;
        WeaponName = weapon.WeaponName;
        DeliveryTimeRemaining = weapon.DeliveryTime;
        IsDelivered = false;
        TotalInfluence = weapon.WarInfluence;
        TotalDuration = weapon.InfluenceDuration;
        InfluenceTimeRemaining = weapon.InfluenceDuration;
    }
}