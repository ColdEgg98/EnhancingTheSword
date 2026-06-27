using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AAResourceManager
{
    public Dictionary<string, AsyncOperationHandle<Sprite>> loadhandles = new();
    private const string PLACEHOLDER_KEY = "Placeholder";
    
    public async UniTask SetSpriteAsync(IViewable viewable, Image targetImage)
{
    if (viewable == null || targetImage == null) return;

    string key = viewable.AddressableKey;
    if (string.IsNullOrEmpty(key)) key = PLACEHOLDER_KEY;

    await LoadSprite(key, targetImage);
}

private async UniTask LoadSprite(string key, Image targetImage)
{
    if (loadhandles.TryGetValue(key, out var cached))
    {
        if (cached.Status == AsyncOperationStatus.Succeeded)
            {
                if (targetImage != null) targetImage.sprite = cached.Result;
            return;
        }

        if (cached.Status == AsyncOperationStatus.Failed)
        {
            Addressables.Release(cached);
            loadhandles.Remove(key);
        }
        else
        {
            await UniTask.WaitUntil(() => cached.IsDone);
                if (cached.Status == AsyncOperationStatus.Succeeded && targetImage != null)
                    targetImage.sprite = cached.Result;
            return;
        }
    }

    var handle = Addressables.LoadAssetAsync<Sprite>(key);
    loadhandles[key] = handle;

    await UniTask.WaitUntil(() => handle.IsDone);

    if (handle.Status == AsyncOperationStatus.Succeeded)
    {
        if (targetImage != null) targetImage.sprite = handle.Result;
    }
    else
    {
        Debug.LogWarning($"❔ [ResourceManager] AA을 찾을 수 없습니다 : {key}");
        Addressables.Release(handle);
        loadhandles.Remove(key);

        // Placeholder로 폴백 (무한 재귀 방지)
        if (key != PLACEHOLDER_KEY)
            await LoadSprite(PLACEHOLDER_KEY, targetImage);
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
    }
}
