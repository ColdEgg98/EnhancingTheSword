using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;

public class GetItemPopupUI : MonoBehaviour
{
    private CanvasGroup cg;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
    }

    public void Init()
    {
        cg.alpha = 1f;
        gameObject.SetActive(true);
    }

    public async Task PlayExitAnimation()
    {
        Sequence seq = DOTween.Sequence();
        seq.Join(cg.DOFade(0f, 2f));
        
        await seq.AsyncWaitForCompletion();
    }
}   
