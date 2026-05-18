using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    private TooltipUI tooltipUI;
    private Button shopButton;
    [SerializeField] private string itemId;

    private string description;

    void Awake()
    {
        tooltipUI = FindAnyObjectByType<TooltipUI>();
        shopButton = GetComponent<Button>();
        MaterialItem item = (MaterialItem)GameManager.Instance.allOfItemsDictionary[itemId];
        description = item.Description;
    }

    void Start()
    {
        ButtonSub();
    }

    private void ButtonSub()
    {
        var pointerDown = shopButton.gameObject.AddComponent<ObservablePointerDownTrigger>().OnPointerDownAsObservable();
        var pointerUp = shopButton.gameObject.AddComponent<ObservablePointerUpTrigger>().OnPointerUpAsObservable();
        var pointerExit = shopButton.gameObject.AddComponent<ObservablePointerExitTrigger>().OnPointerExitAsObservable();

        var cancleStream = Observable.Merge(pointerUp, pointerExit);

        // 0.35초 홀드하면 툴팁 UI의 포지션 변경 후 켜기
        pointerDown
            .SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(.35f)).TakeUntil(cancleStream))
            .Subscribe(_ =>
            {
                Vector3 thisPos = gameObject.GetComponent<RectTransform>().position;
                // ture -> 켜기, td.text -> 툴팁 내용
                tooltipUI.SetUICondition(true, description, thisPos);
            })
            .AddTo(this);

        cancleStream
            .Subscribe(_ => tooltipUI.SetUICondition(false, string.Empty))
            .AddTo(this);
    }
}
