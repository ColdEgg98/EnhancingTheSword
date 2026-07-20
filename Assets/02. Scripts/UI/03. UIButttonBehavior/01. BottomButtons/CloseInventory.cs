using UnityEngine;
using UnityEngine.UI;

public class CloseInventory : MonoBehaviour
{
    [SerializeField] private InventoryPresenter presenter;

    private void Awake()
    {
        presenter = FindAnyObjectByType<InventoryPresenter>();

        GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("버튼 클릭 감지됨.");
            if (presenter.gameObject.activeSelf)
                presenter.CloseInventory();
        });
    }
}
