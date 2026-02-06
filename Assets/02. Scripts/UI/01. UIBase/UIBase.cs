using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public abstract class UIBase : MonoBehaviour
{
    protected virtual CanvasGroup Cg { get; private set; }
    protected virtual Dictionary<EUIRole, TextMeshProUGUI> TextMap { get; private set; }
    protected Dictionary<EUIRole, Image> ImageMap { get; private set; } = new();
    private Dictionary<EUIRole, AsyncOperationHandle<Sprite>> _handleMap = new();
    public abstract Transform InitializeTarget { get; protected set; }

    protected virtual void SetContext()
    {
        Cg = GetComponent<CanvasGroup>();

        // UITextBinder를 프리팹 속 텍스트에 달고 검색해서 사용
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
                if (i != null && !TextMap.ContainsKey(b.role))
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

    public async UniTask<Sprite> GetImage(EUIRole role, string addressableKey)
    {
        // 일단 핸들 비우기
        if (_handleMap.TryGetValue(role, out var h)) {
            Addressables.Release(h);
        }

        // 로드
        try
        {
            AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(addressableKey);

            _handleMap[role] = handle;
            return await handle.ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        catch (UnityEngine.AddressableAssets.InvalidKeyException)
        {
            Debug.LogWarning($"UI 이미지를 찾을 수 없습니다. (Key : {addressableKey})");
            return null;
        }
    }

    protected virtual void OnDestroy()
    {
        foreach (var key in _handleMap.Keys)
        {
            if (_handleMap[key].IsValid())
            {
                Addressables.Release(_handleMap[key]);
            }
        }
        _handleMap.Clear();
    }
}
