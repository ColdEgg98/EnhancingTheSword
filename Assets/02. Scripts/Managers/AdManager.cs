using System;
using GoogleMobileAds.Api;
using UniRx;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    // 테스트용 리워드 광고 단위 ID
#if UNITY_ANDROID || UNITY_EDITOR
    private readonly string _adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IOS
    private readonly string _adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
    private readonly string _adUnitId = "unused";
#endif

    private RewardedAd _rewardedAd;

    // UniRx: 광고 준비 상태 관찰용
    private readonly BoolReactiveProperty _isAdReady = new BoolReactiveProperty(false);
    public IReadOnlyReactiveProperty<bool> IsAdReady => _isAdReady;

    // 상점 광고 쿨타임
    public readonly float AdsCoinCooldownMinutes = 1f;

    // 실제 남은시간
    public ReactiveProperty<TimeSpan> timeRemain = new(TimeSpan.Zero);

    private void Start()
    {
        Debug.Log("✅ [AdManager] : Start");

        // GameManager 등에서 초기화를 제어하고 싶다면 
        // Start 대신 별도의 Init 메서드로 빼서 GameManager에서 호출해도 좋아!
        InitializeAdMob();
    }

    #region Initialize

    private void InitializeAdMob()
    {
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("✅ [AdManager] : AdMob SDK Initialized");
            LoadRewardedAd();
        });
    }

    #endregion

    #region Ad Operations

    public void LoadRewardedAd()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        _isAdReady.Value = false;
        var adRequest = new AdRequest();

        RewardedAd.Load(_adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError($"❌ [AdManager] : 광고 로드 실패 - {error}");
                return;
            }

            Debug.Log("✅ [AdManager] : 리워드 광고 로드 성공");
            _rewardedAd = ad;
            _isAdReady.Value = true;

            RegisterEventHandlers(_rewardedAd);
        });
    }

    public void ShowRewardedAd(Action onRewardGranted)
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"✅ [AdManager] : 광고 시청 완료! 보상 지급 로직 실행");
                onRewardGranted?.Invoke();
            });
        }
        else
        {
            Debug.LogWarning("⚠️ [AdManager] : 광고가 아직 준비되지 않았음. 다시 로드를 시도합니다.");
            LoadRewardedAd();
        }
    }

    #endregion

    #region Event Handlers

    private void RegisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("✅ [AdManager] : 광고 창 닫힘. 백그라운드에서 새 광고 로드 시작");
            LoadRewardedAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError($"❌ [AdManager] : 광고 표시 실패 - {error}");
            LoadRewardedAd();
        };
    }

    #endregion
}