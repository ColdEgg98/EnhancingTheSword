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

    public void TipTextAppend(string s)
    {
        tipList.Add(s);
    }

    public void TipTextSub(string s)
    {
        tipList.Remove(s);
    }
}
