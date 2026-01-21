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
        SetNew();
    }

    public UserData(PreviewData data)
    {
        previewData = data;
        SetNew();
    }

    private void SetNew()
    {
        previewData = new();
        myWeapons = new();
        myWeaponRefs = new();
        myAchievementRefs = new();
        materials = new();
        materialRefs = new();
    }

    // Stat
    public PreviewData previewData;
    public float chanceBonus;
    public long totalGold;
    public int enhanceCount;
    public int failCount;

    // Weapon
    public List<Weapon> myWeapons;
    public List<int> myWeaponRefs;

    // Achievement
    public HashSet<String> myAchievementRefs;

    // Material
    public List<MaterialItem> materials;
    public List<int> materialRefs;
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

public class MaterialItem
{
    public int index { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    // 이미지 추가?
}