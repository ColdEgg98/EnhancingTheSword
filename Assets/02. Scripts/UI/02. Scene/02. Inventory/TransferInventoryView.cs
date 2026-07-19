using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 무기 전송고 인벤토리 스크립트. 인벤토리 드로잉 관리
/// </summary>
public class TransferInventoryView : MonoBehaviour
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

    // 삭제된 인덱스를 매개변수로 받습니다.
    public void RefreshInventory(int deletedIndex)
    {
        // 1. 최신 리스트로 갱신 (리스트 자체는 새로 가져와야 함)
        myWeapons = GameManager.Instance.GetViewableMyWeapons();

        // 2. 부분 갱신 작업 담을 리스트
        List<UniTask> updateTasks = new List<UniTask>();

        // 3. 삭제된 인덱스부터 슬롯의 끝까지만 반복문을 돕니다! (0 ~ deletedIndex-1 은 무시)
        int loopLimit = Mathf.Min(myWeapons.Count + 1, Contents.Length);

        for (int i = deletedIndex; i < loopLimit; i++)
        {
            if (i < myWeapons.Count)
            {
                // 당겨진 데이터가 있다면 이미지 교체 (비동기)
                updateTasks.Add(ImageChange(i, myWeapons));
            }
            else
            {
                // 데이터가 없는 뒷부분 슬롯은 깔끔하게 지워줌 (동기 처리)
                ClearSingleSlot(i);
            }
        }

        // 4. 변경된 부분만 대기 후 적용
        UniTask.WhenAll(updateTasks).Forget();
    }

    // 슬롯 하나만 비움
    private void ClearSingleSlot(int index)
    {
        if (Contents[index] != null)
        {
            Contents[index].color = Color.clear;
            Contents[index].raycastTarget = false;
        }
    }
}
