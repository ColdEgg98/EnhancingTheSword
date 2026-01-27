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
        imageSize = new Vector2(size, size);
        offSet = new Vector2(offSetValue, offSetValue);
        if (redSquareSprite == null)
            redSquareSprite = Resources.Load<Sprite>("Sprite/RedSquare");
    }

    public void Generate(Transform TargetTransform)
    {
        redSquareObje = new GameObject("RedSquare", typeof(Image));
        redSquareImage = redSquareObje.GetComponent<Image>();
        redSquareImage.sprite = redSquareSprite;

        // 우 상단에 앵커 걸기
        RectTransform redDotRect = redSquareImage.GetComponent<RectTransform>();
        redDotRect.anchorMin = new Vector2(1, 1);
        redDotRect.anchorMax = new Vector2(1, 1);
        redDotRect.sizeDelta = imageSize;
        redDotRect.anchoredPosition = offSet;

        redDotRect.SetParent(TargetTransform.transform, false);
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
        Destroy(redSquareObje);
    }
}
