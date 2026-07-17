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
    [SerializeField] private RawImage targetImage;   
    [SerializeField] private Material wipeMaterial;   

    [Header("Timing")]
    [SerializeField] private float inDuration = 0.5f;
    [SerializeField] private float outDuration = 0.5f;
    [SerializeField] private Ease inEase = Ease.OutQuad;
    [SerializeField] private Ease outEase = Ease.InQuad;

    [Header("Shader Property Cache")]
    private static readonly int ProgressId = Shader.PropertyToID("_Progress");
    // [추가] 셰이더의 Wipe Mode 제어를 위한 프로퍼티 ID 캐싱
    private static readonly int WipeModeId = Shader.PropertyToID("_WipeMode");

    private CancellationTokenSource _cts;
    private Material _instancedMaterial;

    private void Awake()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<RawImage>();
        }

        if (wipeMaterial != null)
        {
            _instancedMaterial = Instantiate(wipeMaterial);
            targetImage.material = _instancedMaterial;
        }

        SetProgressImmediate(0f);
        SetVisible(false);
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();

        if (_instancedMaterial != null)
        {
            Destroy(_instancedMaterial);
        }
    }

    /// <summary>
    /// 화면을 패턴으로 덮는다 (우상단 -> 좌하단 방향으로 채워짐)
    /// </summary>
    public async UniTask PlayInAsync(CancellationToken externalToken = default)
    {
        RestartToken(externalToken, out var token);

        SetVisible(true);
        SetProgressImmediate(0f);

        if (_instancedMaterial == null) return;

        // [추가] 트랜지션을 시작하기 전에 셰이더 모드를 In(0)으로 명시적 설정
        _instancedMaterial.SetFloat(WipeModeId, 0f);

        try
        {
            await DOTween
                .To(() => _instancedMaterial.GetFloat(ProgressId),
                    v => _instancedMaterial.SetFloat(ProgressId, v),
                    1f,
                    inDuration)
                .SetEase(inEase)
                .SetLink(gameObject) 
                .ToUniTask(cancellationToken: token);
        }
        catch (OperationCanceledException)
        {
            SetProgressImmediate(1f);
        }
    }

    /// <summary>
    /// 패턴이 걷히며 화면이 드러난다 (우상단 -> 좌하단 방향으로 사라짐)
    /// </summary>
    public async UniTask PlayOutAsync(CancellationToken externalToken = default)
    {
        RestartToken(externalToken, out var token);

        if (_instancedMaterial == null) return;

        // [추가] 트랜지션을 시작하기 전에 셰이더 모드를 Out(1)으로 명시적 설정
        _instancedMaterial.SetFloat(WipeModeId, 1f);

        try
        {
            // Progress는 이전 단계를 이어받아 1에서 시작하여 0으로 떨어집니다.
            // (셰이더 내부에서 modeVal이 1일 때 fixedProg = 1.0 - _Progress 처리를 거쳐
            // 실제 드로잉 비동기 값은 0에서 1로 전진하게 만듭니다.)
            await DOTween
                .To(() => _instancedMaterial.GetFloat(ProgressId),
                    v => _instancedMaterial.SetFloat(ProgressId, v),
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

    public async UniTask PlayTransitionAsync(Action onCovered, CancellationToken externalToken = default)
    {
        await PlayInAsync(externalToken);
        onCovered?.Invoke();
        await PlayOutAsync(externalToken);
    }

    private void SetVisible(bool visible)
    {
        if (targetImage != null)
        {
            targetImage.enabled = visible;
        }
    }

    private void SetProgressImmediate(float value)
    {
        if (_instancedMaterial != null)
        {
            _instancedMaterial.SetFloat(ProgressId, value);
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