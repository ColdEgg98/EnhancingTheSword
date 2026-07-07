using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningDirection : MonoBehaviour
{
    [SerializeField] private TextAsset openingLine;
    [SerializeField] private TextMeshProUGUI lineText;
    [SerializeField] private CanvasGroup coverCanvas;
    [SerializeField] private CanvasGroup textCanvas;

    [Header("Typing Settings")]
    [SerializeField] private float charInterval = 0.05f;
    [SerializeField] private float lineInterval = 1.5f;

    [Header("Fade Settings")]
    [SerializeField] private float fadeInDuration = 1.0f;
    [SerializeField] private float fadeOutDuration = 2.0f;

    [Header("Scene Settings")]
    [SerializeField] private int nextSceneIndex = 3;

    private OpeningData openingData;
    private CancellationTokenSource cts;

    private void Awake()
    {
        cts = new CancellationTokenSource();

        if (openingLine == null)
        {
            Debug.LogError("OpeningLine (TextAsset)이 인스펙터에 할당되지 않았습니다.");
            return;
        }

        openingData = JsonUtility.FromJson<OpeningData>(openingLine.text);

        if (openingData == null || openingData.lines == null)
            Debug.LogError("JSON 파싱에 실패했거나 lines 데이터가 비어있습니다.");

        coverCanvas.alpha = 1f;
        textCanvas.alpha = 0f;
    }

    public void SkipOpening() => cts?.Cancel();

    private void Start()
    {
        if (openingData?.lines != null)
            RunOpeningSequenceAsync().Forget();
    }
    private async UniTaskVoid RunOpeningSequenceAsync()
{
    var token = cts.Token;
    try
    {
        Debug.Log("[Opening] fade-in 시작");
        var fadeInSeq = DOTween.Sequence()
            .Join(coverCanvas.DOFade(0f, fadeInDuration))
            .Join(textCanvas.DOFade(1f, fadeInDuration))
            .SetEase(Ease.InOutSine);
        await fadeInSeq.ToUniTask(cancellationToken: token);
        coverCanvas.blocksRaycasts = false;
        Debug.Log("[Opening] fade-in 종료 → 타이핑 시작");

        await PlayOpeningAsync(token);
        coverCanvas.blocksRaycasts = true;
        Debug.Log("[Opening] 타이핑 종료 → fade-out 시작");

        var fadeOutSeq = DOTween.Sequence()
            .Join(coverCanvas.DOFade(1f, fadeOutDuration))
            .Join(textCanvas.DOFade(0f, fadeOutDuration))
            .SetEase(Ease.InSine);
        await fadeOutSeq.ToUniTask(cancellationToken: token);
        Debug.Log("[Opening] fade-out 종료");
    }
    catch (OperationCanceledException)
    {
        Debug.Log("[Opening] 취소 감지 (스킵)");
    }

    Debug.Log("[Opening] 씬 전환 직전");

        await UniTask.Yield(); // 한 프레임 양보 후 진행
        await SceneManager.LoadSceneAsync(nextSceneIndex).ToUniTask();
    Debug.Log("[Opening] 씬 전환 완료"); // 이게 안 찍히면 LoadSceneAsync 자체 또는 다음 씬 초기화가 원인
}
    private async UniTask PlayOpeningAsync(CancellationToken token)
    {
        lineText.text = string.Empty;

        foreach (string line in openingData.lines)
        {
            await TypeLineAsync(line, token);
            await UniTask.WaitForSeconds(lineInterval, cancellationToken: token);
            lineText.text = string.Empty;
        }
    }

    private async UniTask TypeLineAsync(string line, CancellationToken token)
    {
        lineText.text = string.Empty;

        foreach (char c in line)
        {
            lineText.text += c;
            await UniTask.WaitForSeconds(charInterval, cancellationToken: token);
        }
    }

    private void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }
}

[System.Serializable]
public class OpeningData
{
    public string[] lines;
}