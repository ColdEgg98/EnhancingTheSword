using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActions : MonoBehaviour
{
    private GameControls actions;
    private EnhanceSword enhanceSword;
    private SellWeapon sellWeapon;
    private InventoryButtonBehavior inventoryButtonBehavior;
    private BuyItemButtonBehavior buyItemButtonBehavior;

    void Awake()
    {
        actions = new();
        enhanceSword = FindAnyObjectByType<EnhanceSword>();
        sellWeapon = FindAnyObjectByType<SellWeapon>();
        inventoryButtonBehavior = FindAnyObjectByType<InventoryButtonBehavior>();
        buyItemButtonBehavior = FindAnyObjectByType<BuyItemButtonBehavior>();
    }

    void OnEnable()
    {
        actions.Enable();

        actions.Player.Enhance.performed += OnEnhance;
        actions.Player.Sell.performed += OnSell;
        actions.Player.Inventory.performed += OnInventory;
        actions.Player.BuyItem.performed += OnBuyItem;
    }

    private void OnEnhance(InputAction.CallbackContext context)
    {
        if (!enhanceSword.isEnhancing)
            enhanceSword.RunEnhancing().Forget();
        else
            Debug.Log("❌ 현재 강화중입니다.");
    }

    private void OnSell(InputAction.CallbackContext context)
    {
        if (sellWeapon.isDictHasData)
            sellWeapon.SetPanel(sellWeapon.inventoryData.IViewableForSell);
        else
            sellWeapon.SetPanel();
    }

    private void OnInventory(InputAction.CallbackContext context)
    {
        inventoryButtonBehavior.OnClickButton().Forget();

    }

    private void OnBuyItem(InputAction.CallbackContext context)
    {
        buyItemButtonBehavior.OpenPanel();
    }


    private void OnDisable()
    {
        actions.Player.Enhance.performed -= OnEnhance;
        actions.Player.Sell.performed -= OnSell;
        actions.Player.Inventory.performed -= OnInventory;

        actions.Disable();
    }
}
