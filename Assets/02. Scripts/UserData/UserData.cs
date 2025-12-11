using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

[Serializable]
public class UserData
{
    public UserData()
    {
        previewData = new();
        myWeapons = new();
        myWeaponRefs = new();
        myAchivementRefs = new();
    }

    public UserData(PreviewData data)
    {
        previewData = data;
        myWeapons = new();
        myWeaponRefs = new();
        myAchivementRefs = new();
    }

    public PreviewData previewData;
    public int test;
    public List<Weapon> myWeapons;
    public List<int> myWeaponRefs;
    public List<String> myAchivementRefs;
    // items
}

[Serializable]
public class PreviewData
{
    public bool isUsed;
    public int slotNumber;
    public int achivementCount;
    public string time;
    public long goldRef;
}

[Serializable]
public class WrapperForPreviewData
{
    public PreviewData[] slots;
}

[Serializable]
public class Weapon
{
    public int index { get; set; }
    public string name { get; set; }
    public long price { get; set; }
    public float probability { get; set; }
    public long enhancingPrice { get; set; }
    public string addressID { get; set; }
    public List<int> needItems { get; set; }
    private Sprite sprite;

    public async Awaitable<Sprite> GetWeaponSpriteAsync()
    {
        AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(addressID);
        sprite = await handle.Task;
        return sprite;
    }
    public async Task ApplySpriteToImage(Image targetImage)
    {
        // 내부에서 await로 풀어서 처리
        Sprite sprite = await GetWeaponSpriteAsync();

        if (targetImage != null)
        {
            targetImage.sprite = sprite;
        }
    }
}

[Serializable]
public class Achivement : IDisposable
{
    public string achivementID { get; set; }
    public string description { get; set; }
    public float probabilityPlus { get; set; }
    private AsyncOperationHandle<Sprite> _handle;
    public async Task AchivementSpriteApply(Image targetImage)
    {
        if (_handle.IsValid())
            Addressables.Release(_handle);

        _handle = Addressables.LoadAssetAsync<Sprite>(achivementID);
        await _handle.Task;
        targetImage.sprite = _handle.Result;
    }

    // 사용하는 UI에서 Dispose를 잘해줘야함.
    // 업적 이미지가 내려갈 때 Dispose를 호출하게되거나
    // 업적 Monobehavior 상속된 핸들러 클래스를 만들어서 업적 UI에 붙이면될듯
    // 만약 이 솔루션으로 되면 dispose대신 ondestory 쓰면 되고
    public void Dispose()
    {
        if (_handle.IsValid())
            Addressables.Release(_handle);
        _handle = default;
    }
}