using UnityEngine;

public class StrikePointData : MonoBehaviour
{
    [SerializeField] private RectTransform gague;
    [SerializeField] private RectTransform point;
    private float randomX;
    private float halfPointWidth => point.rect.width / 2f;
    private float speedRatio = 1f;
    private float cursorSpeed;

    void Awake()
    {
        SetRandomPlace();
    }

    public void Init()
    {
        SetRandomPlace();
    }

    private void SetRandomPlace()
    {
        SetValues();

        point.anchoredPosition = new Vector2(randomX, point.anchoredPosition.y);
    }

    private void SetValues()
    {
        float gagueWidth = gague.rect.width;

        float minX = -gagueWidth / 2f * halfPointWidth;
        float maxX =  gagueWidth / 2f * halfPointWidth;

        randomX = Random.Range(minX, maxX);

        cursorSpeed = halfPointWidth * 2 * speedRatio;
    }

    public (float randomX, float halfWidth, float cursorSpeed) GetPointValues()
    {
        SetValues();

        return (randomX, halfPointWidth, cursorSpeed);
    }
}
