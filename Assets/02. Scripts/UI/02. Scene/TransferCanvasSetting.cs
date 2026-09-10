using UnityEngine;

public class TransferCanvasSetting : MonoBehaviour
{
    void Start()
    {
        Canvas TransferCanva = GetComponent<Canvas>();
        TransferCanva.gameObject.SetActive(false);
        TransferCanva.sortingOrder = 3;
    }
}
