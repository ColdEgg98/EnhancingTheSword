using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActions : MonoBehaviour
{
    public GameControls Actions { get; private set; }

    private EnhanceSword enhanceSword;
    private SellWeapon sellWeapon;
    private InventoryButtonBehavior inventoryButtonBehavior;
    private BuyItemButtonBehavior buyItemButtonBehavior;

    void Awake()
    {
        Actions = new GameControls();

        enhanceSword = FindAnyObjectByType<EnhanceSword>();
        sellWeapon = FindAnyObjectByType<SellWeapon>();
        inventoryButtonBehavior = FindAnyObjectByType<InventoryButtonBehavior>();
        buyItemButtonBehavior = FindAnyObjectByType<BuyItemButtonBehavior>();
    }

    void OnEnable()
    {
        // 🚨 중요: 전체를 Enable() 하지 마시고, 시작할 때 필요한 맵만 켭니다.
        SwitchToForgeMap();

        // Forge 액션 이벤트 구독
        Actions.Forge.Enhance.performed += OnEnhance;
        Actions.Forge.Sell.performed += OnSell;
        Actions.Forge.Inventory.performed += OnInventory;
        Actions.Forge.BuyItem.performed += OnBuyItem;
    }

    // ==========================================
    // 맵 스위칭(전환) 메서드
    // ==========================================
    public void SwitchToMiniGameMap()
    {
        Actions.Forge.Disable();    // 대장간 조작 비활성화 (미니게임 중 UI 조작 방지)
        Actions.MiniGame.Enable();  // 미니게임 조작 활성화
        Debug.Log("🎮 조작계 전환: MiniGame");
    }

    public void SwitchToForgeMap()
    {
        Actions.MiniGame.Disable(); // 미니게임 조작 비활성화
        Actions.Forge.Enable();     // 대장간 조작 다시 활성화
        Debug.Log("🔨 조작계 전환: Forge");
    }
    // ==========================================

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

    void OnDisable()
    {
        // 구독 해제
        Actions.Forge.Enhance.performed -= OnEnhance;
        Actions.Forge.Sell.performed -= OnSell;
        Actions.Forge.Inventory.performed -= OnInventory;
        Actions.Forge.BuyItem.performed -= OnBuyItem;

        Actions.Disable();
    }
}