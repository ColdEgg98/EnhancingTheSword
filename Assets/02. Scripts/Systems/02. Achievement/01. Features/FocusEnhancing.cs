using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class FocusEnhancing : MonoBehaviour
{
    [SerializeField] private GameObject obj;
    [SerializeField] private Button button;
    private bool isFocusing;
    private const string achievementID = "ReachLevel10";

    private void Awake()
    {
        button.onClick.AddListener(() => FocusButtonBehavior());
        isFocusing = false;
        if (!IsUnLocked())
            Sub();
    }

    private bool IsUnLocked()
    {
        if (GameManager.Instance.currentData.myAchievementRefs.Contains(achievementID))
        {
            obj.gameObject.SetActive(true);
            return true;
        }
        return false;
    }

    private void Sub()
    {
        GameManager.Instance.userDataManager.featureCode
            .Subscribe(code =>
            {
                if (code == 10)
                    UnLockFocusEnhancing();
            })
            .AddTo(this);
    }

    private void UnLockFocusEnhancing()
    {
        obj.gameObject.SetActive(true);
        GameManager.Instance.ShowToast("[해금] : 집중 강화");
    }

    private void FocusButtonBehavior()
    {
        RectTransform rect = obj.gameObject.GetComponent<RectTransform>();
        isFocusing = !isFocusing;
        GameManager.Instance.isFocusOn.Value = isFocusing;

        if (isFocusing)
            GameManager.Instance.redSquareManager.ResizableRedSquareGenerater(rect, 170f, -75f);
        else
            GameManager.Instance.redSquareManager.RedSquareRemover(rect);
    }
}
