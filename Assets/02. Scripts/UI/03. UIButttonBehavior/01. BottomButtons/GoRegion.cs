using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GoRegion : MonoBehaviour
{
    [SerializeField] private Canvas targetCanva;
    [SerializeField] private PlayPatternWipe playPatternWipe;
    private NowRegion currentCanva;
    private Button btn;
    public Action closeInven;

    void Awake()
    {
        currentCanva = GetComponentInParent<NowRegion>();

        btn = GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            _ = MoveAndPlayPattern();
            closeInven?.Invoke();
        });
    }

    void Start()
    {
        //wipe는 Awake단계에서 SetActive되기 때문
        if (playPatternWipe == null)
            playPatternWipe = FindAnyObjectByType<PlayPatternWipe>();
    }

    private async UniTaskVoid MoveAndPlayPattern()
    {
        if (currentCanva.nowCanvas != targetCanva)
        {
            await playPatternWipe.PlayTransitionAsync(MoveRegion);
        }
        else if (currentCanva.nowCanvas == targetCanva)
        {
            Debug.LogWarning($"nowCanvas({currentCanva.nowCanvas})와 targetCanva({targetCanva})가 같습니다.");
        }
    }

    private void MoveRegion()
    {
        targetCanva.gameObject.SetActive(true);
        currentCanva.nowCanvas.gameObject.SetActive(false);

        currentCanva.nowCanvas = targetCanva;
    }
}
