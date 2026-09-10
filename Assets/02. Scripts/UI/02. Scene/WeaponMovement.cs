using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WeaponMovement : MonoBehaviour
{
    [Header("무기")]
    private float moveValue;
    private float duration;
    private Image weapon;
    private float originY;

    private void Awake()
    {
        weapon = GetComponentInChildren<Image>();
        moveValue = 10f;
        duration = 3f;
        originY = weapon.transform.localPosition.y;
    }

    private void Start()
    {
        WeaponMoveUp();
    }

    private void WeaponMoveUp()
    {
        weapon.transform.DOLocalMoveY(originY + moveValue, duration)
            .SetEase(Ease.OutSine)
            .OnComplete(WeaponMoveDown);
    }

    private void WeaponMoveDown()
    {
        weapon.transform.DOLocalMoveY(originY - moveValue, duration)
            .SetEase(Ease.OutSine)
            .OnComplete(WeaponMoveUp);
    }
}
