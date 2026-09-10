using System.Collections.Generic;
using UniRx;
using UnityEngine;

// 무기 출하 슬롯 표기를 코딩한 클래스
public class SlotGridView : MonoBehaviour
{
    public int slotIndex;
    public ReactiveCollection<WeaponSupplySlotButton> slots;
    [SerializeField] private List<GameObject> slotbacks;

    void Awake()
    {
        slots = GetComponentsInChildren<WeaponSupplySlotButton>(true).ToReactiveCollection();
        Debug.Log($"출하 가능 slot 갯수 : {GameManager.Instance.shippingSlot.Value}");

        // 가용한 슬롯 갯수만큼 오브젝트 켜줌
        SetActiveSlots();
        SetActiveWhenAddSlot();
    }

    private void SetActiveSlots()
    {
        int slotcount = GameManager.Instance.shippingSlot.Value;

        for (int i = 0; i < slotcount; i++)
        {
            slots[i].gameObject.SetActive(true);
            slotbacks[i].SetActive(true);
        }
    }

    private void SetActiveWhenAddSlot()
    {
        GameManager.Instance.shippingSlot
            .Subscribe(s =>
            {
                slots[s - 1].gameObject.SetActive(true);
                slotbacks[s - 1].gameObject.SetActive(true);
            })
            .AddTo(this);
    }
}
