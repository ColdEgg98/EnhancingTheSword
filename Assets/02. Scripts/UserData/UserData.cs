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
        myWeaponRefs = new();
        myWeapons = new();
        previewData = new();
    }

    public UserData(PreviewData data)
    {
        previewData = data;
        myWeaponRefs = new();
        myWeapons = new();
        previewData = new();
    }

    public PreviewData previewData;
    public int test;
    public List<Weapon> myWeapons;
    public List<String> myWeaponRefs;
    // items
}

[Serializable]
public class PreviewData
{
    public bool isUsed;
    public int slotNumber;
    public int achivementCount;
    public string time;
    public long gold;
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
    public double price { get; set; }
    public float probability { get; set; }
    public double enhancingPrice { get; set; }
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