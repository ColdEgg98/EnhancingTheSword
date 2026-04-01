using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 컨텐츠 클릭 됐을 떄 출하 대기 슬롯에 등록.
/// 각 개체의 이름이 숫자로 되있기 떄문에 인덱스로 사용한다.
/// </summary>
public class SupplyItemButton : MonoBehaviour
{
    private Button button;
    private int slotNum;

    // event
    private Subject<(int slotNumber, int weaponIndex)> _onClickSlot = new();
    public IObservable<(int slotNumber, int weaponIndex)> onClickSlot => _onClickSlot;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonBehaviour);
    }

    private void ButtonBehaviour()
    {
        slotNum = GetComponentInParent<OpenSlotNum>().currentNum;
        if (!int.TryParse(gameObject.name, out int index))
        {
            Debug.LogError($"[SupplyItemButton] : gameObject의 이름 변경할 수 없음 '{gameObject.name}'");
        }
        _onClickSlot.OnNext((slotNumber : slotNum, weaponIndex : index)); // UI에 띄우기 요청
        
        GameManager.Instance.TryDeliverWeapon(int.Parse(gameObject.name));
    }

    void OnDestroy()
    {
        _onClickSlot.OnCompleted();
    }
}
