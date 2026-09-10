using UnityEngine;
using UnityEngine.UI;

public class TransferXButton : MonoBehaviour
{
    [SerializeField] GameObject view;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => view.SetActive(false));
    }
}
