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
            if (currentCanva.nowCanvas != targetCanva)
            {
                targetCanva.gameObject.SetActive(true);
                currentCanva.nowCanvas.gameObject.SetActive(false);
                
                currentCanva.nowCanvas = targetCanva;
            }
            else if (currentCanva.nowCanvas == targetCanva)
            {
                Debug.LogWarning($"nowCanvas({currentCanva.nowCanvas})와 targetCanva({targetCanva})가 같습니다.");
            }
        });
    }
}
