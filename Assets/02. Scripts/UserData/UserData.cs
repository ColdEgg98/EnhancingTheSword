using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class UserData
{
    public UserData() { }

    public UserData(PreviewData data)
    {
        previewData = data;
    }

    public PreviewData previewData;
    public int test;
    public List<Weapon> myWeapons;
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
    public int index { get; set; }
    public string name {  get; set; }
    public double price { get; set; }
    public float probability { get; set; }
    public double enhancingPrice { get; set; }
    public string addressID { get; set; }
    public List<int> needItems { get; set; }
}