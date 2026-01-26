using UnityEngine;
using UnityEngine.InputSystem;

public class InputActions : MonoBehaviour
{
    private GameControls actions;
    private EnhanceSword enhanceSword;
    private SellWeapon sellWeapon;
    private InventoryButtonBehavior inventoryButtonBehavior;

    void Awake()
    {
        actions = new();
        enhanceSword = FindAnyObjectByType<EnhanceSword>();
        sellWeapon = FindAnyObjectByType<SellWeapon>();
        inventoryButtonBehavior = FindAnyObjectByType<InventoryButtonBehavior>();
    }

    void OnEnable()
    {
        actions.Enable();

        actions.Player.Enhance.performed += OnEnhance;
        actions.Player.Sell.performed += OnSell;
        actions.Player.Inventory.performed += OnInventory;
    }

    private void OnEnhance(InputAction.CallbackContext context)
    {
        enhanceSword.RunEnhancing();
    }

    private void OnSell(InputAction.CallbackContext context)
    {
        sellWeapon.SetPanel();
    }

    private void OnInventory(InputAction.CallbackContext context)
    {
        inventoryButtonBehavior.OnClickButton();
    }

    private void OnDisable()
    {
        actions.Player.Enhance.performed -= OnEnhance;
        actions.Player.Sell.performed -= OnSell;
        actions.Player.Inventory.performed -= OnInventory;

        actions.Disable();
    }
}
