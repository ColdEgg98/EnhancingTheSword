using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 컨텐츠 클릭 됐을 떄 출하 대기 슬롯에 등록.
/// </summary>
public class SupplyItemButton : MonoBehaviour
{
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonBehaviour);
    }

    private void ButtonBehaviour()
    {
        GameManager.Instance.TryDeliverWeapon(int.Parse(gameObject.name));
    }
}
