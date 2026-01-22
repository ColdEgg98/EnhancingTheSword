using System;
using System.Collections.Generic;

[Serializable]
public class Achievement
{
    // Info
    public string AchivementID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    // Reward
    public RewardType RewardType { get; set; }
    public float Value { get; set; }

    // Condition
    public ConditionType ConditionType { get; set; }
    public long ConditionValue { get; set; }

    public Dictionary<EUIRole, string> GetTextData()
    {
        return new Dictionary<EUIRole, string>
        {
            { EUIRole.Title, Name },
            { EUIRole.Description, Description}
        };
    }
}

public enum RewardType
{
    Gold,
    Weapon,
    Item,
    ProbabilityBonus,
    End
}

public enum ConditionType
{
    GameStart,
    GameEnd,
    ShotEnhance,
    MaxLevel,
    FailEnhance,
    GoldAmount,
    TotalGold,
    End
}