using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class LRArrowBehavior : MonoBehaviour
{
    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;

    private void Awake()
    {
        rightButton.onClick.AddListener(() => ArrowBehavior(1));
        leftButton.onClick.AddListener(() => ArrowBehavior(-1));
        Subscribe();
    }

    private void ArrowBehavior(int value)
    {
        GameManager.Instance.soundManager.PlaySFX("Click");
        
        if (GameManager.Instance.selectWeaponIndex.Value >= 0)
            GameManager.Instance.selectWeaponIndex.Value += value;
        
        else
            GameManager.Instance.selectWeaponIndex.Value = 0; // 판매 후 -2로 빠지기 때문에
    }

    private void Subscribe()
    {
        GameManager.Instance.selectWeaponIndex
            .CombineLatest(
                GameManager.Instance.currentData.myWeapons.ObserveCountChanged(notifyCurrentCount: true),
                (index, count) => (index, count))
            .Subscribe(t => UpdateButtons(t.index, t.count))
            .AddTo(this);
    }

    private void UpdateButtons(int index, int count)
    {
        rightButton.interactable = index < count - 1;
        leftButton.interactable = index > 0;
    }
}