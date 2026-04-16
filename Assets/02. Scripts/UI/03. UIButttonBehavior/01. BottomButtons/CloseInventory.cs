using UnityEngine;
using UnityEngine.UI;

public class CloseInventory : MonoBehaviour
{
    [SerializeField] private InventoryView Inventory;

    private void Awake()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            Debug.Log("버튼 클릭 감지됨.");
            if (Inventory.gameObject.activeSelf) Inventory.CloseTheInventory();
        });
    }
}
