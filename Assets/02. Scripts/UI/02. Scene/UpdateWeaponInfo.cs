using UnityEngine;
using UnityEngine.UI;

public class UpdateWeaponInfo : MonoBehaviour
{
    private SceneHandler SceneHandler;

    private void Awake()
    {
        SceneHandler = FindAnyObjectByType<SceneHandler>();
        Weapon weapon = GameManager.Instance.currentWeapon.Value;
        GetComponent<Button>().onClick.AddListener(() => SceneHandler.UpdateWeaponInfo(weapon));
    }
}
