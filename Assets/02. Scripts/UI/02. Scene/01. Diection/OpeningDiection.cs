using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // 1. 씬 전환을 위해 추가

public class OpeningDirection : MonoBehaviour
{
    [SerializeField] private TextAsset openingLine;
    [SerializeField] private TextMeshProUGUI lineText;
    [SerializeField] private CanvasGroup coverCanvas;
    [SerializeField] private CanvasGroup textCanvas;

    [Header("Typing Settings")]
    [SerializeField] private float charInterval = 0.05f;   // 글자 간격 (초)
    [SerializeField] private float lineInterval = 1.5f;    // 줄 사이 대기 (초)

    [Header("Fade Settings")]
    [SerializeField] private float fadeInDuration = 1.0f;  // 오프닝 시작 페이드
    [SerializeField] private float fadeOutDuration = 2.0f; // 오프닝 종료 페이드

    [Header("Scene Settings")]
    [SerializeField] private int nextSceneIndex = 3;       // 2. 이동할 씬의 빌드 인덱스 (씬 3)
    // 만약 씬 이름으로 관리하고 싶다면 아래 주석을 해제해서 사용하세요.
    // [SerializeField] private string nextSceneName = "Scene3"; 

    private OpeningData openingData;

    private void Awake()
    {
        if (openingLine == null)
        {
            Debug.LogError("OpeningLine (TextAsset)이 인스펙터에 할당되지 않았습니다.");
            return;
        }

        openingData = JsonUtility.FromJson<OpeningData>(openingLine.text);

        if (openingData == null || openingData.lines == null)
        {
            Debug.LogError("JSON 파싱에 실패했거나 lines 데이터가 비어있습니다.");
        }

        coverCanvas.alpha = 1f;
        textCanvas.alpha = 0f;
    }

    private void Start()
    {
        if (openingData != null && openingData.lines != null)
        {
            RunOpeningSequenceAsync().Forget();
        }
    }

    private async UniTaskVoid RunOpeningSequenceAsync()
    {
        // 1. coverCanvas 페이드 아웃 + textCanvas 페이드 인 동시 진행
        Sequence fadeInSeq = DOTween.Sequence();
        _ = fadeInSeq.Join(coverCanvas.DOFade(0f, fadeInDuration));
        _ = fadeInSeq.Join(textCanvas.DOFade(1f, fadeInDuration));
        _ = fadeInSeq.SetEase(Ease.InOutSine);

        await fadeInSeq.ToUniTask(cancellationToken: destroyCancellationToken);

        // 2. 타이핑 연출
        await PlayOpeningAsync();

        // 3. 오프닝 끝 — coverCanvas 페이드 인으로 화면 다시 덮기
        Tween fadeOutTween = coverCanvas.DOFade(1f, fadeOutDuration).SetEase(Ease.InSine);
        await fadeOutTween.ToUniTask(cancellationToken: destroyCancellationToken);

        // 4. 연출 종료 후 씬 전환 진행
        await LoadNextSceneAsync();
    }

    private async UniTask PlayOpeningAsync()
    {
        lineText.text = string.Empty;

        foreach (string line in openingData.lines)
        {
            await TypeLineAsync(line);
            await UniTask.WaitForSeconds(lineInterval, cancellationToken: destroyCancellationToken);
            lineText.text = string.Empty;
        }
    }

    private async UniTask TypeLineAsync(string line)
    {
        lineText.text = string.Empty;

        foreach (char c in line)
        {
            lineText.text += c;
            await UniTask.WaitForSeconds(charInterval, cancellationToken: destroyCancellationToken);
        }
    }

    // 3. 비동기 씬 로드 메서드
    private async UniTask LoadNextSceneAsync()
    {
        // SceneManager.LoadSceneAsync를 UniTask로 변환하여 await 처리
        await SceneManager.LoadSceneAsync(nextSceneIndex).ToUniTask(cancellationToken: destroyCancellationToken);
    }
}

[System.Serializable]
public class OpeningData
{
    public string[] lines;
}