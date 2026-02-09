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

    public static string ColorText(string message, string colorCode = "<color=#FF0000>")
    {
        return $"{colorCode}{message}</color>";
    }

    public static string AttachJoSa(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError($"{name}값이 적절하지 않습니다.");
            return string.Empty;
        }

        // 유니코드 한글 범위: '가'(0xAC00) ~ '힣'(0xD7A3)
        char lastChar = name[name.Length - 1];
        if (lastChar >= 0xAC00 && lastChar <= 0xD7A3)
        {
            int code = lastChar - 0xAC00;
            int jong = code % 28; // 종성(받침) 여부
            return jong == 0 ? name + "를" : name + "을";
        }
        else
        {
            // 한글이 아닐 경우 기본적으로 '를' 붙이기
            return name + "를";
        }
    }
}
