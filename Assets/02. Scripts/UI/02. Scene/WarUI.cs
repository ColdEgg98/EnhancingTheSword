using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class WarUI : MonoBehaviour
{
    [SerializeField] private Slider warGauge;
    [SerializeField] private TextMeshProUGUI stageText;

    async UniTask Start()
    {
        await Sub();
    }

    private async UniTask Sub()
    {
        await UniTask.WaitUntil(() => GameManager.Instance != null
                                    && GameManager.Instance.warManager != null);

        Debug.Log("WarGauge Linked");

        GameManager.Instance.warManager.WarGaugeFloat
            .Subscribe(gauge => UpdateGaugeUI(gauge))
            .AddTo(this);

        GameManager.Instance.warManager.CurrentStage
            .Subscribe(stage => UpdateStageText(stage))
            .AddTo(this);
    }


    private void UpdateGaugeUI(float warGauge)
    {
        this.warGauge.value = warGauge / 100;
    }

    private void UpdateStageText(int stage)
    {
        stageText.text = $"현재 스테이지 : {stage}";
    }
}
