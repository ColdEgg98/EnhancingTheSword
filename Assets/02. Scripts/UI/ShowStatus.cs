using TMPro;
using UniRx;
using UnityEngine;

public class ShowStatus : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI goldText;
    string format;

    private void Awake()
    {   
        GameManager.Instance.gold
        .Select(gold => ToWonFormat(gold))
        .Subscribe(formattedGold =>
        {
            goldText.text = formattedGold;
        })
        .AddTo(this);
    }

    private string ToWonFormat(long gold)
    {
        long jo = gold / 1000000000000;
        gold %= 1000000000000;
        long eok = gold / 100000000;
        gold %= 100000000;
        long man = gold / 10000;
        gold %= 10000;

        format = "소지금 : ";
        if (jo > 0)
            format += $"{jo}조 ";
        if (eok > 0)
            format += $"{eok}억 ";
        if (man > 0)
            format += $"{man}만 ";
        if (gold > 0)
            format += gold;
        format += "원";

        return format;
    }
}
