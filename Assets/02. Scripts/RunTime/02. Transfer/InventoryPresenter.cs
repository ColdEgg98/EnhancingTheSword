using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class InventoryPresenter : MonoBehaviour
{
    [SerializeField] private InventoryView view;

    private ReactiveCollection<WeaponSupplySlotButton> _slots;

    void Start()
    {
        _slots = FindObjectsByType<WeaponSupplySlotButton>(FindObjectsSortMode.None).ToReactiveCollection();
        Debug.Log($"출하 가능 slot 갯수 : {GameManager.Instance.shippingSlot.Value}");
        ForeachSlots();
        Sub();
    }

    private void ForeachSlots()
    {
        foreach(var s in _slots)
        {
            SubscribeSlot(s);
        }
    }

    private void Sub()
    {
        _slots.ObserveAdd()
            .Subscribe(s =>
            {
                SubscribeSlot(s.Value);
            })
            .AddTo(this);
    }

    private void SubscribeSlot(WeaponSupplySlotButton s)
    {
        // s에게 구독 '이벤트 호출 시 DrawStart 하도록' 걸기
        s.OnSlotClicked
            .Subscribe(e => view.DrawStart().Forget())
            .AddTo(s)
            .AddTo(this);
    }
}
