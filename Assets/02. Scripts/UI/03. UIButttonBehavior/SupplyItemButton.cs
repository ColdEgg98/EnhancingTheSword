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
    private InventoryView view;

    // event
    private Subject<(int slotNumber, int weaponIndex)> _onClickSlot = new();
    public IObservable<(int slotNumber, int weaponIndex)> onClickSlot => _onClickSlot;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonBehaviour);
        view = FindAnyObjectByType<InventoryView>();
    }

    private void ButtonBehaviour()
    {
        // TODO : UI에 띄우기전에 해당 슬롯이 사용중인지 여부를 확인해야함.

        // 슬롯 그리드를 조회해서 사용할 슬롯 인덱스를 읽어옴
        slotNum = GetComponentInParent<OpenSlotNum>().currentNum;

        // 이 인벤토리 슬롯 버튼의 이름을 사용자 무기 인덱스로 사용
        if (!int.TryParse(gameObject.name, out int index))
        {
            Debug.LogError($"[SupplyItemButton] : gameObject의 이름 변경할 수 없음 '{gameObject.name}'");
        }

        Debug.Log($"현재 클릭된 슬롯 번호: {slotNum}, 무기 인덱스: {index}");

        // 1. 무기 출하 무결성 검사
        //if (!GameManager.Instance.IsVaildDeliverWeapon(index, slotNum)) return;

        // 2. 슬롯 UI에 띄우기 요청
        _onClickSlot.OnNext((slotNumber : slotNum, weaponIndex : index));
        
        // 3. 무기 출하
        //GameManager.Instance.DeliverWeapon(index, slotNum);

        // 4. 인벤토리 최신화
        view.RefreshInventory(index);
    }

    void OnDestroy()
    {
        _onClickSlot.OnCompleted();
    }
}
