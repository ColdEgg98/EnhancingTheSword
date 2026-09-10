using UnityEngine;
using UnityEngine.UI;

public class CloseInventoryForForge : MonoBehaviour
{
    [SerializeField] InventoryView inventory;
    [SerializeField] GameObject inventoryGameObject;

    private void Awake()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            Debug.Log("버튼 클릭 감지됨.");
            if (inventoryGameObject.activeSelf) inventory.HideInventory();
        });
    }
}
