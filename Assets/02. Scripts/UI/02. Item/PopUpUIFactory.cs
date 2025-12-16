using UnityEngine;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;

public class PopUpUIFactory : MonoBehaviour
{
    [SerializeField] private GetItemPopupUI popUpPrefab;
    [SerializeField] private Transform initializeTarget;

    private IObjectPool<GetItemPopupUI> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<GetItemPopupUI>(
            createFunc: () =>
            {
                return Instantiate(popUpPrefab, initializeTarget);
            },
            actionOnGet: obj =>
            {
                obj.transform.SetAsLastSibling();
            },
            actionOnRelease: obj => obj.gameObject.SetActive(false),
            actionOnDestroy: obj => Destroy(obj.gameObject),
            defaultCapacity: 3,
            maxSize: 10
            );
    }

    public void ShowToast(string itemName)
    {
        // Fire and Forget (결과를 기다리지 않고 실행만 함)
        SpawnAndReleaseRoutine(itemName).Forget();
    }

    // 생성 -> 애니메이션 -> 반납 과정을 담당하는 로직
    private async UniTask SpawnAndReleaseRoutine(string itemName)
    {
        // Get
        var item = _pool.Get();
        item.Init(itemName);

        // 애니메이션 실행 및 대기
        await item.PlayExitAnimation();

        // Release
        _pool.Release(item);
    }
}
