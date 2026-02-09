using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AAResourceManager
{
    public Dictionary<string, AsyncOperationHandle<Sprite>> loadhandles = new();

    public async UniTask SetSpriteAsync(IViewable viewable, Image targetImage)
    {
        if (viewable == null || targetImage == null) return;

        string key = viewable.AddressableKey;

        // 캐싱되어 있다면 로드
        if (loadhandles.ContainsKey(key))
        {
            if (loadhandles[key].Status == AsyncOperationStatus.Succeeded)
            {
                targetImage.sprite = loadhandles[key].Result;
                return;
            }
        }

        // 핸들에 키 넣고 로드
        var handle = Addressables.LoadAssetAsync<Sprite>(key);

        // 캐싱 + 핸들 등록
        if (!loadhandles.ContainsKey(key))
            loadhandles.Add(key, handle);

        // Sprite 로드
        try
        {
            Sprite sprite = await handle.ToUniTask();

            if (targetImage != null)
                targetImage.sprite = sprite;
        }
        catch
        {
            Debug.LogWarning($"❔ [ResourceManager] AA을 찾을 수 없습니다 : {key}");
        }
    }

    public async UniTask SetSpriteAsync(string addressKey, Image targetImage)
    {
        Weapon viewableInstance = new();
        viewableInstance.AddressID = addressKey;
        await SetSpriteAsync(viewableInstance, targetImage);
    }

    public void ReleseAllAssets()
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
