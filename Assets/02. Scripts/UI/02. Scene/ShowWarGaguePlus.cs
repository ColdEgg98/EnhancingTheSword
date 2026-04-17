using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;

public class ShowWarGaguePlus : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI warGaguePlus;

    private void Start()
    {
        SubActiveSlot();
    }

    private void SubActiveSlot()
    {
        GameManager.Instance.warManager.Slots
            .ObserveCountChanged()
            .Subscribe(_ => UpdateTotalStatus())
            .AddTo(this);
    }

    // 상태 텍스트를 갱신하는 전용 함수
    private void UpdateTotalStatus()
    {
        // 1. 현재 가동 중인 무기 갯수
        int currentStage = GameManager.Instance.warManager.CurrentStage.Value;

        // 2. 가동 중인 무기들의 영향력 총합 계산 (LINQ의 Sum 활용)
        float totalInfluence = GameManager.Instance.warManager.Slots.Sum(slot => slot.TotalInfluence);

        // 3. 최적화된 TMP 포맷팅으로 UI 갱신 (GC 할당 0)
        // 표기 예시: "가동 중인 무기 : 2 / 총 영향력 : +1.5"
        warGaguePlus.SetText("현재 스테이지 : {0}(+{1})", currentStage, totalInfluence);
    }
}
