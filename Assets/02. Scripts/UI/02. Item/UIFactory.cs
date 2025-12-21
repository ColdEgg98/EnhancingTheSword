using UnityEngine;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;

public class UIFactory : MonoBehaviour
{
    [Header("Prefab Assets")]
    [SerializeField] private List<UIBase> UIBaseprefabs;
    private Dictionary<Type, UIBase> _prefabDict;
    private Dictionary<Type, IObjectPool<UIBase>> _poolDict;
    
    [Header("Monitoring")]
    public int poolDictCounter;

    private void Awake()
    {
        PopulatePrefabDict();
    }

    private void PopulatePrefabDict()
    {
        _poolDict = new();
        _prefabDict = new();

        foreach (var prefab in UIBaseprefabs)
        {
            Type type = prefab.GetType();
            _prefabDict[type] = prefab;

            _poolDict[type] = CreatePool(type);
        }
    }

    private IObjectPool<UIBase> CreatePool(Type type)
    {
        poolDictCounter++;
        Debug.Log($"{type.Name} 타입 pool 생성 및 반환 시도");
        return new ObjectPool<UIBase>(
            createFunc: () => 
            {
                var instance = Instantiate(_prefabDict[type]);
                instance.TryInitTarget();
                instance.gameObject.SetActive(false);
                return instance;
            },
            actionOnGet: ui => 
            {
                ui.gameObject.SetActive(true);
            },
            actionOnRelease: ui => ui.gameObject.SetActive(false),
            actionOnDestroy: ui => Destroy(ui.gameObject),
            defaultCapacity: 1,
            maxSize: 10
        );
    }

    public T Get<T>() where T : UIBase
    {
        var type = typeof(T);

        if (_poolDict.TryGetValue(type, out var pool))
        {
            // BasePopup으로 반환된 것을 T로 캐스팅
            return (T)pool.Get();
        }

        Debug.LogError($"[PopupFactory] Pool not found for type: {type}");
        return null;
    }

    public void Release(UIBase item)
    {
        var type = item.GetType();
        if (_poolDict.TryGetValue(type, out var pool))
        {
            pool.Release(item);
        }
    }

    public void ShowToast(string itemName)
    {
        // Fire and Forget (결과를 기다리지 않고 실행만 함)
        SpawnAndRelease<ToastPopupUI>(itemName).Forget();
    }

    public void ShowNotice(string itemName)
    {
        // Fire and Forget (결과를 기다리지 않고 실행만 함)
        SpawnAndRelease<NoticeUI>(itemName).Forget();
    }

    public void ShowNotice(string itemName, Color c)
    {
        // Fire and Forget (결과를 기다리지 않고 실행만 함)
        SpawnAndRelease<NoticeUI>(itemName, c).Forget();
    }

    // 생성 -> 애니메이션 -> 반납 과정을 담당하는 로직
    private async UniTask SpawnAndRelease<T>(string itemName) where T : UIBase
    {
        // Get
        var item = Get<T>();
        item.Init(itemName);

        // 애니메이션 실행 및 대기
        await item.PlayAnimation();

        // Release
        Release(item);
    }
    
    // 생성 -> 애니메이션 -> 반납 과정을 담당하는 로직
    private async UniTask SpawnAndRelease<T>(string itemName, Color textColor) where T : UIBase
    {
        // Get
        var item = Get<T>();
        item.SetContentText(textColor);
        item.Init(itemName);

        // 애니메이션 실행 및 대기
        await item.PlayAnimation();

        // Release
        Release(item);
    }

}
