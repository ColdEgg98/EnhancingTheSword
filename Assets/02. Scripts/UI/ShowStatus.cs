using System;
using TMPro;
using UnityEngine;

public class ShowStatus : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI goldText;
    private long gold;
    string format;

    private void Awake()
    {
        gold = GameManager.Instance.currentData.previewData.gold;
        ToWonFormat(gold);
    }

    private void ToWonFormat(long gold)
    {
        long jo = gold / 1000000000000;
        gold %= 1000000000000;
        long eok = gold / 100000000;
        gold %= 100000000;
        long man = gold / 10000;
        gold %= 10000;

        format = string.Empty;
        if (jo > 0)
            format += $"{jo}조";
        if (eok > 0)
            format += $"{eok}억";
        if (man > 0)
            format += $"{man}만";
        if (gold > 0)
            format += gold;
    }

    private void Start()
    {
        goldText.text = $"소지금 : {format}원";
    }
}
