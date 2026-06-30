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
            // 1. 페이드 인
            var fadeInSeq = DOTween.Sequence()
                .Join(coverCanvas.DOFade(0f, fadeInDuration))
                .Join(textCanvas.DOFade(1f, fadeInDuration))
                .SetEase(Ease.InOutSine);
            await fadeInSeq.ToUniTask(cancellationToken: token);

            coverCanvas.blocksRaycasts = false;

            // 2. 타이핑 연출
            await PlayOpeningAsync(token);

            coverCanvas.blocksRaycasts = true;

            // 3. 페이드 아웃
            await coverCanvas.DOFade(1f, fadeOutDuration)
                .SetEase(Ease.InSine)
                .ToUniTask(cancellationToken: token);
        }
        catch (OperationCanceledException)
        {
            // 스킵 시 여기서 낙하 → 씬 전환으로 이어짐
        }

        // 4. 스킵이든 정상 완료든 항상 실행
        await SceneManager.LoadSceneAsync(nextSceneIndex).ToUniTask();
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