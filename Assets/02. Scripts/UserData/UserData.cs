using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class UserData
{
    public UserData()
    {
        myAchivementRefs = new();
    }

    public UserData(PreviewData data)
    {
        previewData = data;
        myAchivementRefs = new();
    }

    public PreviewData previewData;
    public int test;
    public List<Weapon> myWeapons;
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
    public long gold;
}

[Serializable]
public class WrapperPreviewData
{
    public PreviewData[] slots;
}

[Serializable]
public class Weapon
{
    public int index;
    public string name;
    public double price;
    public float probability;
    public double enhancingPrice;
    public string addressID;
    public List<int> needItems;
}

[Serializable]
public class Achivement
{
    public string achivementID { get; set; }
    public string description { get; set; }
    public float probabilityPlus { get; set; }
    public Sprite achivementSprite { get; set; }
    public async Task AchivementSpriteApply()
    {
        
    }
}