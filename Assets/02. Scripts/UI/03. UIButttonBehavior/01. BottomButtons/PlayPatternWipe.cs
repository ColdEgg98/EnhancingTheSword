// PatternWipeTransition.cs
//
// PatternWipeTransition.shader 를 제어해서
// "우상단 -> 좌하단 대각선 웨이브로 화면을 패턴으로 덮었다가(In), 다시 걷는(Out)"
// UI 전환을 재생하는 컴포넌트.
//
// 사용 예:
//   await patternWipe.PlayInAsync();     // 화면이 패턴으로 덮임
//   // 이 시점에 실제 패널/씬 교체 로직 실행
//   await patternWipe.PlayOutAsync();    // 패턴이 걷히며 사라짐
//
// 또는 편의 메서드로 한 번에:
//   await patternWipe.PlayTransitionAsync(() => { /* 화면이 덮인 순간 실행할 로직 */ });

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class PlayPatternWipe : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private RawImage targetImage;   // 화면 전체를 덮는 RawImage. 비워두면 자동으로 GetComponent.
    [SerializeField] private Material wipeMaterial;   // PatternWipeTransition 셰이더로 만든 머티리얼 인스턴스

    [Header("Timing")]
    [SerializeField] private float inDuration = 0.5f;
    [SerializeField] private float outDuration = 0.5f;
    [SerializeField] private Ease inEase = Ease.OutQuad;
    [SerializeField] private Ease outEase = Ease.InQuad;

    [Header("Shader Property Cache")]
    private static readonly int ProgressId = Shader.PropertyToID("_Progress");

    // 전환 도중 중복 재생 방지 및 스킵을 위한 토큰
    private CancellationTokenSource _cts;

    private void Awake()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<RawImage>();
        }

        // 머티리얼은 공유 에셋을 직접 건드리지 않도록 런타임에 인스턴스화.
        // (이미 인스펙터에서 인스턴스를 넣어뒀다면 material 대입은 자동으로 인스턴스를 만들어줌)
        if (wipeMaterial != null)
        {
            targetImage.material = wipeMaterial;
        }

        // 시작 시엔 안 보이는 상태로.
        SetProgressImmediate(0f);
        SetVisible(false);
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();

        // 런타임에 생성된 머티리얼 인스턴스 정리 (메모리 누수 방지)
        if (targetImage != null && targetImage.material != null)
        {
            Destroy(targetImage.material);
        }
    }

    /// <summary>
    /// 화면을 패턴으로 덮는다 (Progress 0 -> 1).
    /// </summary>
    public async UniTask PlayInAsync(CancellationToken externalToken = default)
    {
        RestartToken(externalToken, out var token);

        SetVisible(true);
        SetProgressImmediate(0f);

        try
        {
            await DOTween
                .To(() => wipeMaterial.GetFloat(ProgressId),
                    v => wipeMaterial.SetFloat(ProgressId, v),
                    1f,
                    inDuration)
                .SetEase(inEase)
                .SetLink(gameObject) // 오브젝트 파괴 시 트윈 자동 정리 (WebGL 안전)
                .ToUniTask(cancellationToken: token);
        }
        catch (OperationCanceledException)
        {
            // 스킵 등으로 취소된 경우: 즉시 완전히 덮인 상태로 스냅.
            SetProgressImmediate(1f);
        }
    }

    /// <summary>
    /// 덮여있던 패턴이 걷히며 화면이 드러난다 (Progress 1 -> 0).
    /// </summary>
    public async UniTask PlayOutAsync(CancellationToken externalToken = default)
    {
        RestartToken(externalToken, out var token);

        try
        {
            await DOTween
                .To(() => wipeMaterial.GetFloat(ProgressId),
                    v => wipeMaterial.SetFloat(ProgressId, v),
                    0f,
                    outDuration)
                .SetEase(outEase)
                .SetLink(gameObject)
                .ToUniTask(cancellationToken: token);
        }
        catch (OperationCanceledException)
        {
            SetProgressImmediate(0f);
        }
        finally
        {
            SetVisible(false);
        }
    }

    /// <summary>
    /// In -> (화면이 덮인 순간 onCovered 콜백 실행, 보통 씬/패널 교체) -> Out 을 한 번에 처리.
    /// </summary>
    public async UniTask PlayTransitionAsync(Action onCovered, CancellationToken externalToken = default)
    {
        await PlayInAsync(externalToken);
        onCovered?.Invoke();
        await PlayOutAsync(externalToken);
    }

    private void SetVisible(bool visible)
    {
        targetImage.enabled = visible;
    }

    private void SetProgressImmediate(float value)
    {
        if (wipeMaterial != null)
        {
            wipeMaterial.SetFloat(ProgressId, value);
        }
    }

    private void RestartToken(CancellationToken externalToken, out CancellationToken linkedToken)
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken, this.GetCancellationTokenOnDestroy());
        linkedToken = _cts.Token;
    }
}