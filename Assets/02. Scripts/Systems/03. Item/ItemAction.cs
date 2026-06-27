using System;
using System.Collections.Generic;

public interface IItemAction
{
    void Execute(MaterialItem item);
    bool IsValid(MaterialItem item);
}

public class AdsGold : IItemAction
{
    private static readonly Dictionary<int, long> RewardTable = new()
    {
        { 1, 70_000L },
        { 2, 300_000L },
        { 3, 3_500_000L },
        { 4, 7_400_000L },
        { 5, 12_000_000L },
    };

    private const float RandomVariance = 0.10f;

    public void Execute(MaterialItem item)
    {
        GameManager.Instance.adManager.ShowRewardedAd(() =>
        {
            // 광고 시청 완료 후 여기서 보상 지급
            var data = GameManager.Instance.currentData;
            GameManager.Instance.GetGold(CalcReward(data));
            data.shopData.lastAdsGoldTime = DateTime.UtcNow.Ticks;
        });
    }

    public bool IsValid(MaterialItem item)
    {
        float CooldownMinutes = GameManager.Instance.adManager.AdsCoinCooldownMinutes;
        long lastTicks = GameManager.Instance.currentData.shopData.lastAdsGoldTime;
        if (lastTicks == 0) return true;

        var elapsed = DateTime.UtcNow - new DateTime(lastTicks, DateTimeKind.Utc);
        return elapsed.TotalMinutes >= CooldownMinutes;
    }

    private long CalcReward(UserData data)
    {
        int anvilLevel = data.shopData.anvilLevel;

        if (!RewardTable.TryGetValue(anvilLevel, out long baseReward))
            return 0L;

        float variance = UnityEngine.Random.Range(-RandomVariance, RandomVariance);
        long reward = (long)(baseReward * (1f + variance));

        return reward;
    }
}

public class UpgradeAnvil : IItemAction
{
    public void Execute(MaterialItem item)
    {
        GameManager.Instance.currentData.shopData.anvilLevel += 1;
        GameManager.Instance.GetGold(-item.ItemPrice * GameManager.Instance.currentData.shopData.anvilLevel);
    }

    public bool IsValid(MaterialItem item)
    {
        if (GameManager.Instance.currentData.shopData.anvilLevel == 5)
        {
            GameManager.Instance.ShowNotice("이미 최대 레벨입니디.");
            return false;
        }

        if (GameManager.Instance.gold.Value < item.ItemPrice)
        {
            GameManager.Instance.ShowNotice("골드가 부족합니다.");
            return false;
        }

        return true;
    }
}

public class UpgradeHammer : IItemAction
{
    public void Execute(MaterialItem item)
    {
        GameManager.Instance.currentData.chanceBonus += 5;
        GameManager.Instance.currentData.shopData.hammerLevel =+ 1;
        GameManager.Instance.GetGold(-item.ItemPrice * GameManager.Instance.currentData.shopData.hammerLevel);
    }

    public bool IsValid(MaterialItem item)
    {
        if (GameManager.Instance.currentData.shopData.hammerLevel == 3)
        {
            GameManager.Instance.ShowNotice("이미 최대 레벨입니다.");
            return false;
        }

        if (GameManager.Instance.gold.Value < item.ItemPrice)
        {
            GameManager.Instance.ShowNotice("골드가 부족합니다.");
            return false;
        }

        return true;
    }
}

public class LowGradeAntiDestruction : IItemAction
{
    int index;

    public void Execute(MaterialItem item)
    {
        GameManager.Instance.currentWeapon.Value.IsAntiDestruction = true;
        GameManager.Instance.ShowNotice($"하급 강화 파괴 방지 물약을 사용했습니다.");
    }

    public bool IsValid(MaterialItem item)
    {
        index = GameManager.Instance.currentWeapon.Value.Index;

        if (GameManager.Instance.selectWeaponIndex.Value < 0 || GameManager.Instance.currentWeapon.Value == null)
        {
            GameManager.Instance.ShowNotice($"사용할 아이템이 없습니다.");
            return false;
        }

        if (GameManager.Instance.currentWeapon.Value.IsAntiDestruction == true)
        {
            GameManager.Instance.ShowNotice($"이미 사용되었습니다.");
            return false;
        }

        if (index > 8)
        {
            GameManager.Instance.ShowNotice($"강화 단계에 맞지 않는 아이템 입니다.");
            return false;
        }

        return true;
    }
}

public class MiddleGradeAntiDestruction : IItemAction
{
    int index;

    public void Execute(MaterialItem item)
    {
        GameManager.Instance.currentWeapon.Value.IsAntiDestruction = true;
        GameManager.Instance.ShowNotice($"중급 강화 파괴 방지 물약을 사용했습니다.");
    }

    public bool IsValid(MaterialItem item)
    {
        if (GameManager.Instance.selectWeaponIndex.Value < 0 || GameManager.Instance.currentWeapon.Value == null)
        {
            GameManager.Instance.ShowNotice($"사용할 아이템이 없습니다.");
            return false;
        }

        if (GameManager.Instance.currentWeapon.Value.IsAntiDestruction == true)
        {
            GameManager.Instance.ShowNotice($"이미 사용되었습니다.");
            return false;
        }

        index = GameManager.Instance.currentWeapon.Value.Index;
        if (index < 8 || index > 15)
        {
            GameManager.Instance.ShowNotice($"강화 단계에 맞지 않는 아이템 입니다.");
            return false;
        }

        return true;
    }
}

public class HighGradeAntiDestruction : IItemAction
{
    int index;

    public void Execute(MaterialItem item)
    {
        GameManager.Instance.currentWeapon.Value.IsAntiDestruction = true;
        GameManager.Instance.ShowNotice($"상급 강화 파괴 방지 물약을 사용했습니다.");
    }

    public bool IsValid(MaterialItem item)
    {
        index = GameManager.Instance.currentWeapon.Value.Index;

        if (GameManager.Instance.selectWeaponIndex.Value < 0 || GameManager.Instance.currentWeapon.Value == null)
        {
            GameManager.Instance.ShowNotice($"아이템이 없습니다.");
            return false;
        }

        if (GameManager.Instance.currentWeapon.Value.IsAntiDestruction == true)
        {
            GameManager.Instance.ShowNotice($"이미 사용되었습니다.");
            return false;
        }

        if (index < 15)
        {
            GameManager.Instance.ShowNotice($"강화 단계에 맞지 않는 아이템 입니다.");
            return false;
        }

        return true;
    }
}

public class ProbabilityUp : IItemAction
{
    public void Execute(MaterialItem item)
    {
        GameManager.Instance.currentData.chanceBonus += 5f;
        GameManager.Instance.ShowToast("강화 확률이 5% 상승했습니다.");
    }

    public bool IsValid(MaterialItem item)
    {
        return true;
    }
}
