using UnityEngine;

public class CloseInventory : MonoBehaviour
{
    [SerializeField] private InventoryPresenter presenter;
    private GoRegion[] _goRegions;

    private void Awake()
    {
        presenter = FindAnyObjectByType<InventoryPresenter>();

        // 지역 이동 아이콘들에 구독
        _goRegions = GetComponentsInChildren<GoRegion>();
        foreach (var goRegion in _goRegions)
        {
            goRegion.closeInven += RunTrigger;
        }
    }

    private void RunTrigger()
    {
        if (presenter.IsActive)
        {
            Debug.Log("CloseInventory : RunCloseInventory");
            presenter.CloseInventory();
        }
    }
}
