//using Cysharp.Threading.Tasks;
//using System.Linq;
//using TMPro;
//using UniRx;
//using UnityEngine;
//using UnityEngine.UI;

//public class WarUI : MonoBehaviour
//{
//    [SerializeField] private Slider warGauge;
//    [SerializeField] private TextMeshProUGUI stageText;
//    float totalInfluence = 0f;

//    async UniTask Start()
//    {
//        await Sub();
//        SubActiveSlot();
//        totalInfluence = GameManager.Instance.warManager.Slots.Sum(slot => slot.TotalInfluence);
//    }

//    private async UniTask Sub()
//    {
//        await UniTask.WaitUntil(() => GameManager.Instance != null
//                                    && GameManager.Instance.warManager != null);

//        Debug.Log("WarGauge Linked");

//        GameManager.Instance.warManager.WarGaugeFloat
//            .Subscribe(gauge => UpdateGaugeUI(gauge))
//            .AddTo(this);

//        GameManager.Instance.warManager.CurrentStage
//            .Subscribe(stage => UpdateStageText(stage))
//            .AddTo(this);
//    }


//    private void UpdateGaugeUI(float warGauge)
//    {
//        this.warGauge.value = warGauge / 100;
//    }

//    // 현재 영향력 계산
//    private void SubActiveSlot()
//    {
//        GameManager.Instance.warManager.Slots
//            .ObserveCountChanged()
//            .Subscribe(_ =>
//            {
//                totalInfluence = GameManager.Instance.warManager.Slots.Sum(slot => slot.TotalInfluence);
//            })
//            .AddTo(this);
//    }

//    private void UpdateStageText(int stage)
//    {
//        stageText.text = $"현재 스테이지 : {stage}{StrUtiity.ColorText($"(+{totalInfluence})", "<color=#707070>")}";
//    }
//}
