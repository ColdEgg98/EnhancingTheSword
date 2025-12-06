using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponMovement : MonoBehaviour
{
    [Header("무기 이름")]
    [SerializeField] private TextMeshProUGUI weaponName;

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
        // 무기 바뀔 때 text 바뀌어야 하지만 일단 임시
        weaponName.text = GameManager.Instance.currentData.myWeapons[GameManager.Instance.selectWeaponIndex].name;
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
