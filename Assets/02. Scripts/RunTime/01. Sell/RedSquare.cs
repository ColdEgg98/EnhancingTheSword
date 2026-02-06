using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RedSquare : MonoBehaviour
{
    [Header("UI Element")]
    [SerializeField] private Sprite redSquareSprite;
    private Image redSquareImage;

    [Header("Value Setting")]
    public float size = 135f;
    public float offSetValue = -60f;
    private Vector2 imageSize;
    private Vector2 offSet;

    private GameObject redSquareObje;

    private void Awake()
    {
        if (redSquareSprite == null)
            redSquareSprite = Resources.Load<Sprite>("Sprite/RedSquare");
    }

    public async UniTask Generate(Transform TargetTransform)
    {
        imageSize = new Vector2(size, size);
        offSet = new Vector2(offSetValue, offSetValue);

        redSquareObje = new GameObject("RedSquare", typeof(Image));
        redSquareImage = redSquareObje.GetComponent<Image>();
        redSquareImage.raycastTarget = false;
        Color c = redSquareImage.color;
        c.a = 0f;
        redSquareImage.color = c;
        redSquareImage.sprite = redSquareSprite;

        // 우 상단에 앵커 걸기
        RectTransform redDotRect = redSquareImage.GetComponent<RectTransform>();
        redDotRect.anchorMin = new Vector2(1, 1);
        redDotRect.anchorMax = new Vector2(1, 1);
        redDotRect.sizeDelta = imageSize;
        redDotRect.anchoredPosition = offSet;

        redDotRect.SetParent(TargetTransform.transform, false);
        
        await PlayAnimation();
    }
    public async UniTask Generate(Transform TargetTransform, float size, float offSet)
    {
        this.size = size;
        offSetValue = offSet;
        Generate(TargetTransform).Forget();
        await UniTask.CompletedTask;
    }

    public async UniTask PlayAnimation()
    {
        Sequence seq = DOTween.Sequence();
        
        _ = seq.Join(redSquareImage.DOFade(1f, 0.5f).SetEase(Ease.OutBack));
        _ = seq.Join(redSquareImage.transform.DOPunchScale(new Vector2(0.2f, 0.2f), 0.5f, 6, 0.8f));

        await seq.AsyncWaitForCompletion();
    }


    public void SetOffset(float offsetX, float offsetY)
    {
        RectTransform rect = redSquareImage.GetComponent<RectTransform>();
        
        if (rect != null)
        {
            rect.anchoredPosition = new Vector2(offsetX, offsetY);
        }
        else
        {
            Debug.LogWarning("[RedDot] RectTransform이 아직 초기화되지 않았습니다.");
        }
    }
    
    public void Remove()
    {
        Debug.Log($"[RedDot] Run Remove : {gameObject.name}");
        transform.DOKill();
        Destroy(redSquareObje);
    }
}
