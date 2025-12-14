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
        if (gold == 0) return $"0원";

        long jo = gold / 1000000000000;
        gold %= 1000000000000;
        long eok = gold / 100000000;
        gold %= 100000000;
        long man = gold / 10000;
        gold %= 10000;

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        if (jo > 0)
            sb.Append($"{jo}조 ");
        if (eok > 0)
            sb.Append($"{eok}억 ");
        if (man > 0)
            sb.Append($"{man}만 ");
        if (gold >= 0)
            sb.Append($"{gold}");

        sb.Append("원");

        return sb.ToString();
    }
}
