using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;

public class ToastPopupUI : UIBase
{
    public override Transform InitializeTarget { get; protected set; }

    private void Awake()
    {
        SetContext();
    }

    public override void Init(string itemName)
    {
        base.Init(itemName);
        ContentText.text = itemName;
    }

    public override async Task PlayAnimation()
    {
        Sequence seq = DOTween.Sequence();
        seq.Join(Cg.DOFade(0f, 5f).SetEase(Ease.InCubic));
        await seq.AsyncWaitForCompletion();
    }

    protected override void SetInitTarget()
    {
        InitializeTarget = GameObject.FindWithTag("ToastPopupTarget").transform;
    }

}   
