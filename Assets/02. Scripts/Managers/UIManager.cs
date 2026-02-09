using UniRx;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public UIFactory UIFactory;
    public ReactiveCollection<string> tipList { get; private set; } = new();

    // saveSlot에서 호출
    public void Init()
    {
        if (UIFactory == null)
            UIFactory = FindAnyObjectByType<UIFactory>();
    }

    public void TipTextAppend(string s)
    {
        if (!tipList.Contains(s))
            tipList.Add(s);
    }

    public void TipTextSub(string s)
    {
        tipList.Remove(s);
    }
}
