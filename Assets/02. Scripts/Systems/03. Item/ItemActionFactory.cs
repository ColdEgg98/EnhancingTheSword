public static class ItemActionFactory
{
    public static IItemAction ItemFactory(string itemName)
    {
        return itemName.Trim() switch
        {
            "LowGradeAntiDestruction" => new LowGradeAntiDestruction(),
            "MiddleAntiDestruction" => new MiddleGradeAntiDestruction(),
            "HighGradeAntiDestruction" => new HighGradeAntiDestruction(),
            "ProbabilityUp" => new ProbabilityUp(),
            "UpgradeAnvil" => new UpgradeAnvil(),
            "UpgradeHammer" => new UpgradeHammer(),
            "AdsGold" => new AdsGold(),
            _ => null
        };
    }
}