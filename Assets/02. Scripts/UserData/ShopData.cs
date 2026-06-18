using System;

[Serializable]
public class ShopSaveData
{
    public int anvilLevel;
    public int hammerLevel;
    public long lastAdsGoldTime;

    public ShopSaveData()
    {
        anvilLevel = 1;
        hammerLevel = 1;
    }
}
