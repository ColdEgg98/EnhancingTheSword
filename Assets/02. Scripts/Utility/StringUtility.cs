using System.Text;
using TMPro;
using UnityEngine;

public class StrUtiity
{
    public static string ToWonFormat(long gold, string colorCode = "<color=#FFD700>")
    {
        if (gold == 0) return "0골드";

        long AbsoluteValue = System.Math.Abs(gold);

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
            sb.Append($"{colorCode}{AbsoluteValue}</color>");

        sb.Append("골드");

        return sb.ToString();
    }

    private static readonly StringBuilder sb = new StringBuilder(128);

    // 반환값을 string이 아닌 void로 두고, TMP_Text를 직접 조작합니다.
    public static void SetTmpText(TMP_Text tmpText, long gold, string colorCode = "<color=#FFD700>")
    {
        sb.Clear();

        if (gold == 0)
        {
            sb.Append("0골드");
            tmpText.SetText(sb); // StringBuilder를 그대로 넘김 (Zero GC)
            return;
        }

        long absoluteValue = System.Math.Abs(gold);

        long jo = absoluteValue / 1000000000000;
        absoluteValue %= 1000000000000;
        long eok = absoluteValue / 100000000;
        absoluteValue %= 100000000;
        long man = absoluteValue / 10000;
        absoluteValue %= 10000;

        if (jo > 0)
            sb.Append(colorCode).Append(jo).Append("조</color> ");
        if (eok > 0)
            sb.Append(colorCode).Append(eok).Append("억</color> ");
        if (man > 0)
            sb.Append(colorCode).Append(man).Append("만</color> ");
        if (absoluteValue > 0)
            sb.Append(colorCode).Append(absoluteValue).Append("</color>");

        sb.Append("골드");

        // 핵심: .ToString()을 호출하지 않고 StringBuilder 자체를 전달
        tmpText.SetText(sb);
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
