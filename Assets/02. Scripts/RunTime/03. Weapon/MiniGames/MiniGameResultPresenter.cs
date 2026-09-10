using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameResultPresenter : MonoBehaviour
{
    [Header("Result 오브젝트")]
    [SerializeField] private GameObject result;        // 부모 Result 객체
    [SerializeField] private Image resultBack;         // 배경 ResultBack
    [SerializeField] private Image resultImage;        // 자식 ResultImage

    [Header("Addressable 주소")]
    [SerializeField] private string failKey = "MG_fail";
    [SerializeField] private string goodKey = "MG_good";
    [SerializeField] private string perfectKey = "MG_perfect";

    [Header("연출 설정")]
    [SerializeField] private float popDuration = 0.35f;
    [SerializeField] private float holdDuration = 0.7f;
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private Vector3 punchScale = new Vector3(0.2f, 0.2f, 0f);

    [Header("배경 연출 설정")]
    [SerializeField] private float backFadeDuration = 0.2f;
    [SerializeField] private float backMaxAlpha = 1f;
    [SerializeField] private Vector3 backStartScale = new Vector3(1.15f, 1.15f, 1f);

    public async UniTask ShowResult(MiniGameGrade grade)
    {
        string key = GetKey(grade);

        // 스프라이트 로드 및 세팅
        await GameManager.Instance.aAResourceManager.SetSpriteAsync(key, resultImage);

        // 초기 상태 세팅
        result.SetActive(true);

        RectTransform backRt = resultBack.rectTransform;
        RectTransform rt = resultImage.rectTransform;

        // 배경 초기화 (투명 + 살짝 확대)
        backRt.localScale = backStartScale;
        Color backColor = resultBack.color;
        backColor.a = 0f;
        resultBack.color = backColor;

        // 결과 이미지 초기화 (스케일 0 + 불투명)
        rt.localScale = Vector3.zero;
        Color c = resultImage.color;
        c.a = 1f;
        resultImage.color = c;

        // 연출 시퀀스
        Sequence seq = DOTween.Sequence();

        // 1) 배경 등장: 페이드인 + 스케일 정렬 (동시)
        _ = seq.Append(resultBack.DOFade(backMaxAlpha, backFadeDuration));
        _ = seq.Join(backRt.DOScale(1f, backFadeDuration).SetEase(Ease.OutQuad));

        // 2) 결과 이미지 팝업 + 펀치
        _ = seq.Append(rt.DOScale(1f, popDuration).SetEase(Ease.OutBack));
        _ = seq.Append(rt.DOPunchScale(punchScale, 0.25f, 6, 0.8f));

        // 3) 유지
        _ = seq.AppendInterval(holdDuration);

        // 4) 배경 + 결과 이미지 동시 페이드아웃
        _ = seq.Append(resultImage.DOFade(0f, fadeDuration));
        _ = seq.Join(resultBack.DOFade(0f, fadeDuration));

        await seq.ToUniTask();

        result.SetActive(false);
    }

    private string GetKey(MiniGameGrade grade)
    {
        return grade switch
        {
            MiniGameGrade.Perfect => perfectKey,
            MiniGameGrade.Miss => failKey,
            _ => goodKey,
        };
    }
}