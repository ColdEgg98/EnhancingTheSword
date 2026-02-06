using TMPro;
using UniRx;
using UnityEngine;

public class ShowGold : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI goldText;

    private void Awake()
    {   
        GameManager.Instance.gold
        .Select(gold => StrUtiity.ToWonFormat(gold))
        .Subscribe(formattedGold =>
        {
            goldText.text = formattedGold;
        })
        .AddTo(this);
    }
}
