using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class FocusEnhancing : MonoBehaviour
{
    [SerializeField] private GameObject focusButtonObj;
    [SerializeField] private Button button;
    private bool isFocusing;
    private const string achievementID = "Sword of Admiral Yi Sun-sin 1";

    private void Awake()
    {
        button.onClick.AddListener(() => FocusButtonBehavior());
        isFocusing = false;
        focusButtonObj.SetActive(IsUnLocked());
        if (!IsUnLocked())
            Sub();
    }

    private bool IsUnLocked()
    {
        return GameManager.Instance.currentData.myAchievementRefs.Contains(achievementID);
    }

    private void Sub()
    {
        GameManager.Instance.userDataManager.featureCode
            .TakeWhile(_ => !IsUnLocked())
            .Subscribe(code =>
            {
                if (code == 10)
                    UnLockFocusEnhancing();
            })
            .AddTo(this);
    }

    private void UnLockFocusEnhancing()
    {
        focusButtonObj.SetActive(true);
        GameManager.Instance.ShowToast("[해금] : 집중 강화");
    }

    private void FocusButtonBehavior()
    {
        RectTransform rect = focusButtonObj.gameObject.GetComponent<RectTransform>();
        isFocusing = !isFocusing;
        GameManager.Instance.isFocusOn.Value = isFocusing;

        if (isFocusing)
            GameManager.Instance.redSquareManager.ResizableRedSquareGenerater(rect, 170f, -75f);
        else
            GameManager.Instance.redSquareManager.RedSquareRemover(rect);
    }
}
