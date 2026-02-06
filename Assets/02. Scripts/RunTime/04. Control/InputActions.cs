using Cysharp.Threading.Tasks;
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
        if (!enhanceSword.isEnhancing)
            enhanceSword.RunEnhancing().Forget();
        else
            Debug.Log("❌ 현재 강화중입니다.");
    }

    private void OnSell(InputAction.CallbackContext context)
    {
        if (sellWeapon.weaponsForSell != null && sellWeapon.weaponsForSell.Count > 0)
            sellWeapon.SetPanel(sellWeapon.weaponsForSell);
        else
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
