using DG.Tweening;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GetItemPopupUI : MonoBehaviour
{
    private CanvasGroup cg;
    private TextMeshProUGUI contentText;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        contentText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Init(string itemName)
    {
        contentText.text = $"{itemName}을(를) 획득했습니다.";
        cg.alpha = 1f;
        gameObject.SetActive(true);
    }

    public async Task PlayExitAnimation()
    {
        Sequence seq = DOTween.Sequence();
        seq.Join(cg.DOFade(0f, 5f).SetEase(Ease.InCubic));
        
        await seq.AsyncWaitForCompletion();
    }
}   
