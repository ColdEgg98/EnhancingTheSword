using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AAResourceManager
{
    public Dictionary<string, AsyncOperationHandle<Sprite>> loadhandles = new();

    // [추가] UI Image별 최신 요청 상태를 관리하기 위한 토큰 딕셔너리
    private Dictionary<int, CancellationTokenSource> imageLoadingTokens = new();
    private const string PLACEHOLDER_KEY = "Placeholder";

    public async UniTask SetSpriteAsync(IViewable viewable, Image targetImage)
    {
        if (viewable == null || targetImage == null) return;

        // [핵심 1] 1프레임 깜빡임 방지: 새로운 이미지를 로드하기 전에 기존 스프라이트 비우기
        //targetImage.sprite = null;
        targetImage.color = Color.clear;

        string key = viewable.AddressableKey;
        if (string.IsNullOrEmpty(key)) key = PLACEHOLDER_KEY;

        // [핵심 2] 빠른 연타 시 이전 요청 취소
        int imageId = targetImage.GetInstanceID();
        if (imageLoadingTokens.TryGetValue(imageId, out var oldCts))
        {
            oldCts.Cancel();
            oldCts.Dispose();
        }

        // 새로운 토큰 발급 및 저장
        var newCts = new CancellationTokenSource();
        imageLoadingTokens[imageId] = newCts;

        // 로딩 시작 (토큰을 넘겨줌)
        await LoadSprite(key, targetImage, newCts.Token);

        // 로딩이 성공적으로 끝났다면 토큰 딕셔너리 정리
        if (imageLoadingTokens.ContainsKey(imageId) && imageLoadingTokens[imageId] == newCts)
        {
            imageLoadingTokens.Remove(imageId);
            newCts.Dispose();
        }
    }

    // [수정] CancellationToken 매개변수 추가
    private async UniTask LoadSprite(string key, Image targetImage, CancellationToken token)
    {
        if (loadhandles.TryGetValue(key, out var cached))
        {
            if (cached.Status == AsyncOperationStatus.Succeeded)
            {
                // 취소되지 않았다면 적용
                if (!token.IsCancellationRequested && targetImage != null)
                {
                    targetImage.sprite = cached.Result;
                    targetImage.color = Color.white;
                }
                return;
            }

            if (cached.Status == AsyncOperationStatus.Failed)
            {
                Addressables.Release(cached);
                loadhandles.Remove(key);
            }
            else
            {
                // 로딩 중인 캐시 대기 (취소 시 예외 발생을 막기 위해 SuppressCancellationThrow 사용)
                await UniTask.WaitUntil(() => cached.IsDone, cancellationToken: token).SuppressCancellationThrow();

                if (!token.IsCancellationRequested && cached.Status == AsyncOperationStatus.Succeeded && targetImage != null)
                {
                    targetImage.sprite = cached.Result;
                    targetImage.color = Color.white;
                }
                return;
            }
        }

        var handle = Addressables.LoadAssetAsync<Sprite>(key);
        loadhandles[key] = handle;

        await UniTask.WaitUntil(() => handle.IsDone, cancellationToken: token).SuppressCancellationThrow();

        // 취소되었다면 UI에 적용하지 않고 즉시 종료 (캐시는 남겨둠)
        if (token.IsCancellationRequested) return;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            if (targetImage != null)
            {
                targetImage.sprite = handle.Result;
                targetImage.color = Color.white;
            }
        }
        else
        {
            Debug.LogWarning($"❔ [ResourceManager] AA을 찾을 수 없습니다 : {key}");
            Addressables.Release(handle);
            loadhandles.Remove(key);

            // Placeholder로 폴백
            if (key != PLACEHOLDER_KEY)
                await LoadSprite(PLACEHOLDER_KEY, targetImage, token);
            else
                Debug.LogError($"❌ [ResourceManager] Placeholder AA도 찾을 수 없습니다.");
        }
    }

    public async UniTask SetSpriteAsync(string addressKey, Image targetImage)
    {
        Weapon viewableInstance = new();
        viewableInstance.AddressID = addressKey;
        await SetSpriteAsync(viewableInstance, targetImage);
    }

    public void ReleaseAllAssets()
    {
        HashSet<string> keys = new();

        string currentEquipKey = null;
        if (GameManager.Instance.currentWeapon.Value != null) currentEquipKey = GameManager.Instance.currentWeapon.Value.AddressID;

        foreach (var handle in loadhandles)
        {
            // 메인 화면의 무기 릴리즈 방지
            if (currentEquipKey != null && currentEquipKey == handle.Key)
                continue;

            keys.Add(handle.Key);
            Addressables.Release(handle.Value);
        }

        foreach (string key in keys)
        {
            loadhandles.Remove(key);
        }

        // [추가] 에셋 해제 시 실행 중인 렌더링(로딩) 작업들 일괄 취소 처리
        foreach (var cts in imageLoadingTokens.Values)
        {
            cts.Cancel();
            cts.Dispose();
        }
        imageLoadingTokens.Clear();
    }
}