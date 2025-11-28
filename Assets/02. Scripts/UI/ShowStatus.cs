using TMPro;
using UnityEngine;

public class ShowStatus : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI goldText;
    private double goldInt; // 나중에 억 조로 표시하는 기능 추가

    private void Awake()
    {
        goldInt = GameManager.Instance.currentData.previewData.gold;
    }

    private void Start()
    {
        goldText.text = $"소지금 : {goldInt.ToString()}원";
    }
}
