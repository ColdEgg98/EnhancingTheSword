using TMPro;
using UnityEngine;

public class TooltipUI : MonoBehaviour
{
    [SerializeField] private RectTransform ui;
    [SerializeField] private TextMeshProUGUI tmp;
    private Vector3 tooltipOffset = new Vector3(150f, 150f, 0f);


    public void SetUICondition(bool b, string s, Vector3 basePos = default)
    {
        ui.position = basePos + tooltipOffset;
        ui.gameObject.SetActive(b);
        tmp.SetText(s);
    }
}
