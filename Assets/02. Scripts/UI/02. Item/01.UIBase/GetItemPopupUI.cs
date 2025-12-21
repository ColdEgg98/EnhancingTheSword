using DG.Tweening;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ToastPopupUI : UIBase
{
    private CanvasGroup cg;
    private TextMeshProUGUI contentText;
    public override Transform initializeTarget { get; protected set; }


    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        contentText = GetComponentInChildren<TextMeshProUGUI>();
        initializeTarget = GameObject.FindWithTag("ToastPopupTarget").transform;
    }

    public override void Init(string itemName)
    {
        contentText.text = $"{itemName}을(를) 획득했습니다.";
        cg.alpha = 1f;
        gameObject.SetActive(true);
    }

    public override async Task PlayAnimation()
    {
        Sequence seq = DOTween.Sequence();
        seq.Join(cg.DOFade(0f, 5f).SetEase(Ease.InCubic));
        
        await seq.AsyncWaitForCompletion();
    }
}   
