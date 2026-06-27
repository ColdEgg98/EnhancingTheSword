using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIBase : MonoBehaviour
{
    protected virtual CanvasGroup Cg { get; private set; }
    protected virtual Dictionary<EUIRole, TextMeshProUGUI> TextMap { get; private set; }
    protected Dictionary<EUIRole, Image> ImageMap { get; private set; } = new();
    
    public abstract Transform InitializeTarget { get; protected set; }

    protected virtual void SetContext()
    {
        Cg = GetComponent<CanvasGroup>();

        // UITextBinder를 프리팹 속 텍스트/이미지에 달고 검색해서 사용
        UIRoleBinder[] binders = GetComponentsInChildren<UIRoleBinder>(true);
        TextMap = new();
        
        foreach(UIRoleBinder b in binders)
        {
            if (b.role != EUIRole.MainImage)
            {
                var t = b.GetComponent<TextMeshProUGUI>();
                if (t != null && !TextMap.ContainsKey(b.role))
                    TextMap.Add(b.role, t);
            }

            if (b.role == EUIRole.MainImage)
            {
                var i = b.GetComponent<Image>();
                // 기존 코드의 TextMap.ContainsKey 오류를 ImageMap.ContainsKey로 수정했습니다.
                if (i != null && !ImageMap.ContainsKey(b.role)) 
                    ImageMap.Add(b.role, i);
            }
        }

        SetInitTarget();
    }

    // 1개의 TMP가진 UI용
    public virtual void Init(string content)
    {
        Cg.alpha = 1.0f;
        gameObject.SetActive(true);

        TextMap[EUIRole.SoloBody].text = content;
    }

    public virtual void Init(Dictionary<EUIRole, string> dataMap)
    {
        Cg.alpha = 1.0f;
        gameObject.SetActive(true);

        foreach (var dict in dataMap)
        {
            if (TextMap.TryGetValue(dict.Key, out TextMeshProUGUI TMP))
            {
                TMP.text = dict.Value;
            }
        }
    }

    /// <summary>
    /// async를 붙여 사용하세요
    /// </summary>
    public abstract Task PlayAnimation();

    protected abstract void SetInitTarget();

    public virtual bool TryInitTarget()
    {
        Transform initializeTarget = this.InitializeTarget;
        if (initializeTarget)
        {
            this.transform.SetParent(initializeTarget);
            return true;
        }
        else
        {
            return false;
        }
    }

    // 단일 TMP용
    public virtual void SetContentTextColor(Color c)
    {
        TextMap[EUIRole.SoloBody].color = c;
    }

    public virtual void SetContentTextColor(EUIRole role, Color c)
    {
        if (TextMap.ContainsKey(role))
            TextMap[role].color = c;
    }

    /// <summary>
    /// AAResourceManager를 통해 이미지를 세팅합니다.
    /// </summary>
    public async UniTask SetImageAsync(EUIRole role, string addressableKey)
    {
        if (ImageMap.TryGetValue(role, out Image targetImage))
        {
            await GameManager.Instance.aAResourceManager.SetSpriteAsync(addressableKey, targetImage);
        }
        else
        {
            Debug.LogWarning($"[UIBase] {role} 역할이 부여된 Image 컴포넌트를 찾을 수 없습니다.");
        }
    }

    protected virtual void OnDestroy()
    {
        // Addressable 핸들 관리를 AAResourceManager가 담당하므로, 
        // 여기서 핸들을 직접 릴리즈(Addressables.Release)할 필요가 없어졌습니다.
    }
}
