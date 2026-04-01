using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 무기 전송고 인벤토리 스크립트. 인벤토리 드로잉 관리
/// </summary>
public class InventoryView : MonoBehaviour
{
    private List<IViewable> myWeapons;
    [SerializeField] private Image[] Contents;
    private int inventorySize = 20;
    private bool isLoading;

    void Awake()
    {
        isLoading = false;
    }

    public async UniTask DrawStart()
    {
        if (isLoading) return;

        if (!gameObject.activeSelf)
        {
            isLoading = true;
            myWeapons = GameManager.Instance.GetViewableMyWeapons();
            await LoadContents(myWeapons);
            isLoading = false;
        }
        else
            CloseTheInventory();
    }

    public async UniTask LoadContents(List<IViewable> viewAbles)
    {
        // 0. 효과음 재생
        GameManager.Instance.soundManager.PlaySFX("OpenBag");

        int count = viewAbles.Count;

        // 1. 모든 작업을 리스트에 담음
        List<UniTask> loadingTasks = new List<UniTask>();

        for (int i = 0; i < count; i++)
        {
            int index = i;
            // 모든 로딩 순차적 시작, 그 작업들을 리스트에 저장
            loadingTasks.Add(ImageChange(index, viewAbles));
        }

        // 2. 모든 작업이 끝날 때까지 여기서 대기
        await UniTask.WhenAll(loadingTasks);

        // 3. 여기까지 오면 모든 이미지가 100% 로딩 완료된 상태임
        Debug.Log("모든 무기 이미지 로딩 완료");

        gameObject.SetActive(true);
    }

    public async UniTask ImageChange(int index, List<IViewable> IViewables)
    {
        if (index >= inventorySize)
            return;

        await GameManager.Instance.aAResourceManager.SetSpriteAsync(IViewables[index], Contents[index]);
        Contents[index].color = Color.white;
        Contents[index].raycastTarget = true;
    }

    public void RefreshInventory()
    {
        ClearInventory();
        LoadContents(GameManager.Instance.GetViewableMyWeapons()).Forget();
    }

    public void CloseTheInventory()
    {
        ClearInventory();

        myWeapons.Clear();
        GameManager.Instance.aAResourceManager.ReleaseAllAssets();
        gameObject.SetActive(false);
        GameManager.Instance.soundManager.PlaySFX("OpenBag");
    }

    private void ClearInventory()
    {
        // 인벤 내부 정보 리셋
        for (int i = 0; i < Contents.Length; i++)
        {
            int index = i;
            if (Contents[index] == null)
                continue;
            Contents[index].color = Color.clear;
            Contents[index].raycastTarget = false;
        }
    }
}
