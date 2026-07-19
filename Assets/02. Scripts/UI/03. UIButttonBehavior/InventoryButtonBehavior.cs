using UnityEngine;
using UnityEngine.UI;

public class InventoryButtonBehavior : MonoBehaviour
{
    private InventoryPresenter presenter;

    void Start()
    {
        presenter = FindAnyObjectByType<InventoryPresenter>();
        GetComponent<Button>().onClick.AddListener(() => presenter.ToggleInventoryPanel());
    }
}