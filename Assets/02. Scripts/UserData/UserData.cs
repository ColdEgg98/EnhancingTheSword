using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UniRx;

[Serializable]
public class UserData
{
    public UserData()
    {
        SetNew();
    }

    public UserData(PreviewData data)
    {
        SetNew();
        previewData = data;
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
    public long totalGold;
    public int enhanceCount;
    public int failCount;

    // Weapon
    public List<Weapon> myWeapons;
    public List<int> myWeaponRefs;

    // Achievement
    public List<string> myAchievementRefs;
    public float chanceBonus;
    public float addtionalGold;

    // Material
    public List<MaterialItem> materials;
    public List<string> materialRefs;

    // War
    public int shippingSlotRef; // 저장 & 불러오기에만 호출
    public IntReactiveProperty shippingSlot; // 실제 사용
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
public class Weapon : IViewable
{
    public int Index { get; set; }
    public string WeaponName { get; set; }
    public long WeaponPrice { get; set; }
    public float Probability { get; set; }
    public long EnhancingPrice { get; set; }
    public string AddressID { get; set; }
    public float WarInfluence { get; set; }
    public float InfluenceDuration { get; set; }
    public float DeliveryTime { get; set; }
    public List<int> NeedItems { get; set; }


    private bool _isAntiDestruction;

    public bool IsAntiDestruction
    {
        get { return _isAntiDestruction; }
        set { _isAntiDestruction = value; }
    }

    public string AddressableKey => AddressID;

    public string IViewableName => WeaponName;

    public long IViewablePrice => WeaponPrice;
}

public class MaterialItem : IViewable
{
    public string ItemName { get; set; }
    public string Description { get; set; }
    public bool IsConsumable { get; set; }
    [JsonIgnore] public IItemAction action;
    public string AddressID { get; set; }
    public long ItemPrice { get; set; }
    private string _actionString;
    public string ActionString
    {
        get
        {
            return _actionString;
        }
        set
        {
            _actionString = value;
            action = ItemActionFactory.ItemFactory(value);
        }
    }

    public string AddressableKey => AddressID;

    public string IViewableName => ItemName;

    public long IViewablePrice => ItemPrice;
}

public interface IViewable
{
    string AddressableKey { get; }
    string IViewableName { get; }
    long IViewablePrice { get; }
}
