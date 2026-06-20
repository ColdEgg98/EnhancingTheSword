using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

// 버튼 쿨타임 UI 표시용
public class ShowAdCoinTime : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cooldownTimeText;
    private ReactiveProperty<TimeSpan> timeRemain;
    private Button btn;

    private void Start()
    {
        timeRemain = GameManager.Instance.adManager.timeRemain;
        btn = GetComponent<Button>();
        Subscribe();
        GetRemainingCooldown();
        StartCooldownTick();
    }

    // 남은 시간 표기
    private void Subscribe()
    {
        timeRemain
            .Subscribe(ts => {
                //GC 부하 감소
                cooldownTimeText.SetText("{0:00}:{1:00}", (float)ts.Minutes, (float)ts.Seconds);
                btn.interactable = ts <= TimeSpan.Zero;
            })
            .AddTo(this);
    }

    // 초기값 세팅
    public void GetRemainingCooldown()
    {
        float CooldownMinutes = GameManager.Instance.adManager.AdsCoinCooldownMinutes;
        long lastTicks = GameManager.Instance.currentData.shopData.lastAdsGoldTime;
        if (lastTicks == 0) return;

        var elapsed = DateTime.UtcNow - new DateTime(lastTicks, DateTimeKind.Utc);
        var remaining = TimeSpan.FromMinutes(CooldownMinutes) - elapsed;
        timeRemain.Value = remaining < TimeSpan.Zero ? TimeSpan.Zero : remaining;
    }

    // 초기값이 세팅되어있으니 1초씩만 빼도록함
    private void StartCooldownTick()
    {
        Observable.Interval(TimeSpan.FromSeconds(1))
            .TakeWhile(_ => timeRemain.Value > TimeSpan.Zero)
            .Subscribe(_ =>
            {
                var next = timeRemain.Value - TimeSpan.FromSeconds(1);
                timeRemain.Value = next < TimeSpan.Zero ? TimeSpan.Zero : next;
            })
            .AddTo(this);
    }

}
