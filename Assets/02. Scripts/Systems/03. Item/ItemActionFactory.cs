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
            _ => null
        };
    }
}