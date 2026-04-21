using UnityEngine;

public class StrikePointView : MonoBehaviour
{
    [Header("RectTransform")]
    [SerializeField] private RectTransform gauge;
    [SerializeField] private RectTransform point;

    [Header("Cursor")]
    [SerializeField] private RectTransform cursor;

    private float randomX;

    // Rect Widths
    private float halfPointWidth => point.rect.width / 2f;
    private float gaugeWidth => gauge.rect.width;
    private float leftWidth;
    private float rightWidth;

    // Speed & State
    private float speedRatio = 10f;
    private float cursorSpeed;
    private int currentDirection = 1;

    public float hitPoint { get; private set; } // 외부에서는 읽기만 가능하도록 캡슐화

    public void Init()
    {
        SetRandomPlace();
        // 커서 초기 위치 셋팅 (가장 왼쪽)
        hitPoint = -gaugeWidth / 2f;
        UpdateCursorPosition();
    }

    private void SetRandomPlace()
    {
        leftWidth = -gaugeWidth / 2f + halfPointWidth;
        rightWidth = gaugeWidth / 2f - halfPointWidth;

        randomX = Random.Range(leftWidth, rightWidth);
        point.anchoredPosition = new Vector2(randomX, point.anchoredPosition.y);

        cursorSpeed = halfPointWidth * 2 * speedRatio;
    }

    public (float randomX, float halfWidth) GetPointValues()
    {
        return (randomX, halfPointWidth);
    }

    private void Update()
    {
        // 값 이동
        UpdateValue();
        // 실제 UI 반영
        UpdateCursorPosition();
    }

    private void UpdateValue()
    {
        // 1. 방향 설정 (게이지의 양 끝에 도달하면 방향 반전)
        float maxTravel = gaugeWidth / 2f;

        if (hitPoint <= -maxTravel)
            currentDirection = 1;
        else if (hitPoint >= maxTravel)
            currentDirection = -1;

        // 2. 값 이동
        hitPoint += cursorSpeed * currentDirection * Time.deltaTime;
    }

    private void UpdateCursorPosition()
    {
        cursor.anchoredPosition = new Vector2(hitPoint, cursor.anchoredPosition.y);
    }
}