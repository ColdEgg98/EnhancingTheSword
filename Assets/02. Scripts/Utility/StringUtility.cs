using System.Text;
using UnityEngine;

public class StrUtiity
{
    public static string ToWonFormat(long gold, string colorCode = "<color=#FFD700>")
    {
        if (gold == 0) return "0골드";

        long AbsoluteValue = (long)Mathf.Abs(gold);

        long jo = AbsoluteValue / 1000000000000;
        AbsoluteValue %= 1000000000000;
        long eok = AbsoluteValue / 100000000;
        AbsoluteValue %= 100000000;
        long man = AbsoluteValue / 10000;
        AbsoluteValue %= 10000;

        StringBuilder sb = new();

        if (jo > 0)
            sb.Append($"{colorCode}{jo}조</color> ");
        if (eok > 0)
            sb.Append($"{colorCode}{eok}억</color> ");
        if (man > 0)
            sb.Append($"{colorCode}{man}만</color> ");
        if (AbsoluteValue > 0)
            sb.Append($"{colorCode}{gold}</color>");

        sb.Append("골드");

        return sb.ToString();
    }

}
