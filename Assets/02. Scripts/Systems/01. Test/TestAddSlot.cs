using UnityEngine;
using UnityEngine.UI;

public class TestAddSlot : MonoBehaviour
{
    private Button btn;
    void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            GameManager.Instance.currentData.shippingSlot.Value++;
            Debug.Log($"슬롯 증가 : {GameManager.Instance.currentData.shippingSlot}(+{1})");
        });
    }
}
