using UnityEngine;
using UnityEngine.UI;

public class QuickCloseInventory : MonoBehaviour
{
    private InventoryPresenter _presenter;

    private void Awake()
    {
        _presenter = FindAnyObjectByType<InventoryPresenter>();
        GetComponent<Button>().onClick.AddListener(() => _presenter.CloseInventory());
    }
}
