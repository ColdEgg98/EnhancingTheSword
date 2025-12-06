using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponMovement : MonoBehaviour
{
    [Header("무기 이름")]
    [SerializeField] private TextMeshProUGUI weaponName;

    [Header("무기")]
    [SerializeField] private float moveValue = 10f;
    [SerializeField] private float duration = 3f;
    private Image weapon;

    private void Awake()
    {
        weapon = GetComponentInChildren<Image>();
    }

    private void Start()
    {
        // 무기 바뀔 때 text 바뀌어야 하지만 일단 임시
        weaponName.text = GameManager.Instance.currentData.myWeapons[GameManager.Instance.selectWeaponIndex].addressID;
        WeaponMoveUp();
    }

    private void WeaponMoveUp()
    {
        transform.DOMoveY(weapon.transform.position.y + moveValue, duration)
            .SetEase(Ease.OutSine)
            .OnComplete(WeaponMoveDown);
    }

    private void WeaponMoveDown()
    {
        transform.DOMoveY(weapon.transform.position.y - moveValue, duration)
            .SetEase(Ease.OutSine)
            .OnComplete(WeaponMoveUp);
    }
}
