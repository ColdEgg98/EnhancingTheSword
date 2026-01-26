public class StrUtiity
{
    public static string ToWonFormat(long gold)
    {
        if (gold == 0) return "0골드";

        long jo = gold / 1000000000000;
        gold %= 1000000000000;
        long eok = gold / 100000000;
        gold %= 100000000;
        long man = gold / 10000;
        gold %= 10000;

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        if (jo > 0)
            sb.Append($"<color=#FFD700>{jo}조</color> ");
        if (eok > 0)
            sb.Append($"<color=#FFD700>{eok}억</color> ");
        if (man > 0)
            sb.Append($"<color=#FFD700>{man}만</color> ");
        if (gold > 0)
            sb.Append($"<color=#FFD700>{gold}</color>");

        sb.Append("골드");

        return sb.ToString();
    }

}
