using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System;

public class BuyWoodSword : MonoBehaviour
{
    Button _btn;

    void Awake()
    {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(BuySword);

        _btn.OnPointerDownAsObservable()
            .SelectMany(_ =>
                Observable.Timer(TimeSpan.FromSeconds(.5f), TimeSpan.FromSeconds(0.1f))
                .TakeUntil(_btn.OnPointerUpAsObservable())
            )
            .Subscribe(_ =>
            {
                BuySword();
            })
            .AddTo(this);
    }

    private void BuySword()
    {
        if (GameManager.Instance.currentData.myWeapons.Count < 20)
        {
            long swordPrice = 10000;
            if (GameManager.Instance.gold.Value < swordPrice)
            {
                GameManager.Instance.uiManager.UIFactory.ShowNotice("골드가 부족합니다.", Color.white);
                return;
            }
            // 진동 피드백
            Handheld.Vibrate();

            GameManager.Instance.gold.Value -= swordPrice;
            GameManager.Instance.currentData.myWeapons.Add(GameManager.Instance.allOfWeaponDictionary[1]);
            GameManager.Instance.ShowToast($"목검을 구매했습니다.\n{StrUtiity.ToWonFormat(swordPrice)}를 사용했습니다.");
        }
        else
        {
            GameManager.Instance.uiManager.UIFactory.ShowNotice("인벤토리가 가득 찼습니다.");
        }
    }
}
