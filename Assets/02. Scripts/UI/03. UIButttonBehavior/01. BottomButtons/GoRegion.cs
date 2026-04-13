using UnityEngine;
using UnityEngine.UI;

public class GoRegion : MonoBehaviour
{
    [SerializeField] private Canvas targetCanva;
    private NowRegion currentCanva;
    private Button btn;

    void Awake()
    {
        currentCanva = GetComponentInParent<NowRegion>();

        btn = GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            if (currentCanva != targetCanva)
            {
                targetCanva.gameObject.SetActive(true);
                currentCanva.gameObject.SetActive(false);
                
                currentCanva.nowCanvas = targetCanva;
            }
        });
    }
}
