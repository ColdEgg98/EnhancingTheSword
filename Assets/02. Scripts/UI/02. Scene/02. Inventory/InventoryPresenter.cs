using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPresenter : MonoBehaviour
{
    [SerializeField] private InventoryView weaponView;
    [SerializeField] private InventoryView itemView;

    private class Tab
    {
        public InventoryView View;
        public InventoryModel Model;
        public bool Dirty = true;     // 최초엔 로딩 필요
        public bool Loading;          // 중복 로딩 방지
    }

    private Tab[] tabs;
    private int activeIndex = 0;
    private Tab Active => tabs[activeIndex];

    // CloseInventory에서 사용
    public bool IsActive => Active.View.IsPanelActive;

    private void Awake()
    {
        tabs = new[]
        {
            new Tab { View = weaponView, Model = new WeaponInventoryModel() },
            new Tab { View = itemView,   Model = new MaterialInventoryModel() },
        };

        foreach (var t in tabs)
        {
            var tab = t; // 클로저 캡처용 지역 변수

            tab.View.OnCloseClicked          += CloseInventory;
            tab.View.OnSwitchCategoryClicked += () => SwitchCategory().Forget();
            tab.View.OnSlotClicked           += HandleSlotClick;
            tab.View.OnItemSwapped           += HandleSlotSwap;

            // 원본 컬렉션이 바뀌면 그 탭을 dirty로 → 다음에 열 때 자동 재로딩
            tab.Model.ObserveChanged()
                .Subscribe(_ => tab.Dirty = true)
                .AddTo(this);
        }
    }

    // 인스펙터 버튼/단축키용
    public void ToggleInventoryPanel()
    {
        if (!Active.View.IsPanelActive) OpenAsync().Forget();
        else CloseInventory();
    }

    private async UniTaskVoid OpenAsync()
    {
        GameManager.Instance.soundManager.PlaySFX("OpenBag");

        await EnsureLoadedAsync(activeIndex);   // 활성 탭 먼저 (사용자 대기 최소화)
        Active.View.ShowInventory();

        // 나머지 탭은 백그라운드 프리로드 → 첫 전환도 즉시
        for (int i = 0; i < tabs.Length; i++)
            if (i != activeIndex) EnsureLoadedAsync(i).Forget();
    }

    // dirty한 탭만 실제 AA 로딩. 이미 로드됐거나 로딩 중이면 그에 맞게 처리.
    private async UniTask EnsureLoadedAsync(int index)
    {
        var tab = tabs[index];

        if (tab.Loading)                        // 이미 로딩 중이면 끝날 때까지 대기
        {
            await UniTask.WaitUntil(() => !tab.Loading);
            return;
        }
        if (!tab.Dirty) return;                 // 캐시 유효 → 즉시 반환 (전환이 빨라지는 핵심)

        tab.Loading = true;
        try
        {
            tab.Model.RefreshItems();
            tab.View.ClearSlots();

            var tasks = new List<UniTask>();
            int count = Mathf.Min(tab.Model.CurrentItems.Count, 20); // 최대 20슬롯 방어
            for (int i = 0; i < count; i++)
                tasks.Add(LoadSlotImageAsync(tab, i));

            await UniTask.WhenAll(tasks);
            tab.Dirty = false;
        }
        finally
        {
            tab.Loading = false;
        }
    }

    private async UniTask LoadSlotImageAsync(Tab tab, int index)
    {
        IViewable item = tab.Model.CurrentItems[index];
        Image slot = tab.View.GetSlotImage(index);
        await GameManager.Instance.aAResourceManager.SetSpriteAsync(item, slot);
        tab.View.SetupSlot(index, tab.Model.PreserveAspect);
    }

    private async UniTaskVoid SwitchCategory()
    {
        int prev = activeIndex;
        activeIndex = (activeIndex + 1) % tabs.Length;

        tabs[prev].View.SetPanelActive(false);  // 이전 탭 끄기 (로딩 상태 유지)
        await EnsureLoadedAsync(activeIndex);    // 프리로드 됐으면 즉시, 아니면 이때 한 번 로딩
        Active.View.SetPanelActive(true);        // 팁 텍스트는 건드리지 않음
    }

    private void HandleSlotClick(int index)
    {
        Active.Model.OnSlotSelected(index);      // 무기 선택 or 재료 사용
        CloseInventory();
        // 목록 변화(재료 사용 등)는 닫을 때 전체 dirty로 커버됨
    }

    private void HandleSlotSwap(int origin, int target)
    {
        if (!Active.Model.CanReorder) return;    // 재료 탭은 여기서 걸러짐

        // 데이터 순서 교환이 성공한 경우에만 스프라이트를 즉시 교환 (AA 재로딩 없음)
        if (Active.Model.Reorder(origin, target))
            Active.View.SwapSlotSprites(origin, target);
    }

    public void CloseInventory()
    {
        Active.View.HideInventory();             // 현재 보이는 탭만 닫기(팁 정리 포함)

        // 완전히 닫을 때만 에셋 해제 → 두 탭 모두 dirty
        GameManager.Instance.aAResourceManager.ReleaseAllAssets();
        for (int i = 0; i < tabs.Length; i++) tabs[i].Dirty = true;

        activeIndex = 0;                          // 다음엔 무기 탭부터 (원본 동작 유지)
        GameManager.Instance.soundManager.PlaySFX("OpenBag");
    }
}