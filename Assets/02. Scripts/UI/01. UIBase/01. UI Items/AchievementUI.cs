using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class AchievementUI : UIBase
{
    [SerializeField] private Image icon;
    private RectTransform rect;
    private float rectHeight;

    public override Transform InitializeTarget {get; protected set;}

    void Awake()
    {
        SetContext();
        rect = GetComponent<RectTransform>();
        rectHeight = rect.rect.height;
    }

    public async UniTask ShowSequence(Dictionary<EUIRole, string> data, string addKey)
    {
        base.Init(data);

        await SetImageAsync(EUIRole.MainImage, addKey);

        await PlayAnimation();
    }

    public override async Task PlayAnimation()
    {
        rect.anchoredPosition = new Vector2(1, -rectHeight);

        Sequence seq = DOTween.Sequence();
        _ = seq.SetLink(gameObject);
        _ = seq.Append(rect.DOAnchorPosY(0, 1.0f).SetEase(Ease.OutCubic));
        _ = seq.AppendInterval(3.0f);
        _ = seq.Append(rect.DOAnchorPosY(-rectHeight, 1.5f).SetEase(Ease.InCubic));
        await seq.AsyncWaitForCompletion();
    }

    protected override void SetInitTarget()
    {
        InitializeTarget = GameObject.FindWithTag("AchievementTarget").transform;
    }
}
