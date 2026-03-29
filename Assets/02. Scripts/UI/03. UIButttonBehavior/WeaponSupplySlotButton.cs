using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

// 인벤토리 열어줌
public class WeaponSupplySlotButton : MonoBehaviour
{
    [SerializeField] private GameObject InventoryView;
    private Button slotButton;
    [SerializeField] private int num = 0;
    private OpenSlotNum slotNum;

    // Event
    private Subject<int> _onSlotClicked = new();
    public IObservable<int> OnSlotClicked => _onSlotClicked;

    void Awake()
    {
        slotButton = GetComponent<Button>();
        slotNum = InventoryView.GetComponent<OpenSlotNum>();

        slotButton.onClick.AddListener(ClickTheSlot);
    }

    private void ClickTheSlot()
    {
        bool isThis = slotNum.currentNum == num;

        if (InventoryView.activeSelf && isThis)
        {
            CallView();
        }

        else if (InventoryView.activeSelf && isThis == false)
        {
            slotNum.currentNum = num;
            CallView();
        }

        else if (InventoryView.activeSelf == false)
        {
            Debug.Log("클릭 슬롯 분기 : View 꺼져있음");
            CallView();
        }

        else
        {
            Debug.LogWarning("[WeaponSupplySlotButton] : 예기치 못 한 분기 발생함");
        }
    }

    private void CallView()
    {
        Debug.Log("CallView 요청됨");
        _onSlotClicked.OnNext(num);
    }

    private void OnDestroy()
    {
        _onSlotClicked.OnCompleted();
    }
}
