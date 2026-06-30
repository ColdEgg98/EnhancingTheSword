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
        GameManager.Instance.selectWeaponIndex.Value += value;
        GameManager.Instance.soundManager.PlaySFX("Click");
    }

    private void Subscribe()
    {
        GameManager.Instance.selectWeaponIndex
            .Subscribe(index =>
            {
                int count = GameManager.Instance.currentData.myWeapons.Count - 1;

                rightButton.interactable = index + 1 <= count;
                leftButton.interactable = index - 1 >= 0;
            })
            .AddTo(this);
    }
}
