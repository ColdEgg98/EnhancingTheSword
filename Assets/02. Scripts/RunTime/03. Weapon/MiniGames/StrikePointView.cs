using System;
using TMPro;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class StrikePointView : MonoBehaviour
{
    [Header("RectTransform")]
    [SerializeField] private RectTransform gauge;
    [SerializeField] private RectTransform point;

    [Header("Cursor")]
    [SerializeField] private RectTransform cursor;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;

    private float randomX;

    // Rect Widths
    private float halfPointWidth => point.rect.width / 2f;
    private float gaugeWidth => gauge.rect.width;
    private float leftWidth;
    private float rightWidth;

    // Timer
    private FloatReactiveProperty timeRemain = new FloatReactiveProperty(5f);
    private Subject<bool> _isTimeOver = new();
    public IObservable<bool> isTimeOver => _isTimeOver;

    // Speed & State
    private float speedRatio = 10f;
    private float cursorSpeed;
    private int currentDirection = 1;

    public float hitPoint { get; private set; }

    private void Awake()
    {
        timeRemain
            .Subscribe(t =>
            {
                timerText.SetText("{0:2}초", timeRemain.Value);

                if (t <= 0)
                {
                    _isTimeOver.OnNext(true);
                }
            })
            .AddTo(this);
    }

    private void OnEnable()
    {
        SetRandomPlace();
        // 커서 초기 위치 셋팅 (가장 왼쪽)
        hitPoint = -gaugeWidth / 2f;
        timeRemain.Value = 5f;
        UpdateUI();
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
        // 값 적용
        UpdateValue();
        // 실제 UI 반영
        UpdateUI();
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

        // 3. 타이머 계산
        timeRemain.Value -= Time.deltaTime;
    }

    private void UpdateUI()
    {
        // Cursor
        cursor.anchoredPosition = new Vector2(hitPoint, cursor.anchoredPosition.y);
    }
}