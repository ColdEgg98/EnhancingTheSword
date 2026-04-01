using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SlotPresenter : MonoBehaviour
{
    [SerializeField] private SlotGridView slotView;

    [SerializeField] Transform ItemContents;
    private SupplyItemButton[] supplyItemButtons;
    private ReactiveCollection<WeaponSupplySlotButton> _slots;

    void Awake()
    {
        _slots = slotView.slots;
        supplyItemButtons = ItemContents.GetComponentsInChildren<SupplyItemButton>();
        SubForSupplyButtons();
    }

    private void SubForSupplyButtons()
    {
        foreach (var b in supplyItemButtons)
        {
            AgreeCallSlot(b);
        }
    }

    private void AgreeCallSlot(SupplyItemButton item)
    {
        item.onClickSlot
            .Subscribe(e =>
            {
                Image image = _slots[e.slotNumber].GetComponent<Image>();
                image.color = Color.white;
                image.raycastTarget = true;

                IViewable tempViewable = GameManager.Instance.currentData.myWeapons[e.weaponIndex];
                GameManager.Instance.aAResourceManager.
                    SetSpriteAsync(tempViewable, _slots[e.slotNumber].slotImage).Forget();
            })
            .AddTo(this)
            .AddTo(item);
    }
}
