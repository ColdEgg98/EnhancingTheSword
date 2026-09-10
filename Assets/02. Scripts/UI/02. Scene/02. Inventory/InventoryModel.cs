using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public enum EInvenCategory { Weapon, Item }

public abstract class InventoryModel
{
    public abstract EInvenCategory Category { get; }
    public abstract bool PreserveAspect { get; }   // 기존 !isWeaponCategory
    public abstract bool CanReorder { get; }       // 드래그 스왑 허용 여부

    public List<IViewable> CurrentItems { get; protected set; } = new();

    public abstract void RefreshItems();
    public abstract void OnSlotSelected(int index);           // 무기=선택, 재료=사용

    // 원본 컬렉션이 바뀌면 방출 → Presenter가 해당 탭을 dirty로 표시
    public abstract IObservable<Unit> ObserveChanged();

    // 순서 교환 성공 시 true (성공한 경우에만 뷰에서 스프라이트를 교환)
    public virtual bool Reorder(int origin, int target) => false;

    // ReactiveCollection의 모든 변경(추가/삭제/치환/이동/리셋)을 하나로 병합
    protected static IObservable<Unit> ObserveAnyChange<T>(ReactiveCollection<T> c)
        => Observable.Merge(
            c.ObserveAdd().AsUnitObservable(),
            c.ObserveRemove().AsUnitObservable(),
            c.ObserveReplace().AsUnitObservable(),
            c.ObserveMove().AsUnitObservable(),
            c.ObserveReset().AsUnitObservable());
}

public class WeaponInventoryModel : InventoryModel
{
    public override EInvenCategory Category => EInvenCategory.Weapon;
    public override bool PreserveAspect => false;
    public override bool CanReorder => true;

    // myWeapons는 ReactiveCollection<Weapon>
    private ReactiveCollection<Weapon> Source => GameManager.Instance.currentData.myWeapons;

    public override void RefreshItems()
        => CurrentItems = Source.OfType<IViewable>().ToList();

    public override void OnSlotSelected(int index)
    {
        Debug.Log($"인덱스 변경 {GameManager.Instance.selectWeaponIndex.Value} -> {index}");
        GameManager.Instance.selectWeaponIndex.Value = index;
    }

    public override IObservable<Unit> ObserveChanged() => ObserveAnyChange(Source);

    public override bool Reorder(int origin, int target)
    {
        var weapons = Source;
        if (origin < 0 || origin >= weapons.Count) return false;
        if (target < 0 || target >= weapons.Count) return false;
        if (origin == target) return false;

        // 원본 컬렉션 스왑 (ReactiveCollection → ObserveReplace 2회 발생)
        (weapons[origin], weapons[target]) = (weapons[target], weapons[origin]);

        // CurrentItems도 동기화해 다음 Refresh 전까지 일관성 유지
        if (origin < CurrentItems.Count && target < CurrentItems.Count)
            (CurrentItems[origin], CurrentItems[target]) = (CurrentItems[target], CurrentItems[origin]);

        // 선택된 무기 인덱스 따라가기
        int sel = GameManager.Instance.selectWeaponIndex.Value;
        if (origin == sel) GameManager.Instance.selectWeaponIndex.Value = target;
        else if (target == sel) GameManager.Instance.selectWeaponIndex.Value = origin;

        return true;
    }
}

public class MaterialInventoryModel : InventoryModel
{
    public override EInvenCategory Category => EInvenCategory.Item;
    public override bool PreserveAspect => true;
    public override bool CanReorder => false;

    // materials 타입을 몰라도 되게 IList로 접근 (List/ReactiveCollection 모두 호환)
    private IList<MaterialItem> Source => GameManager.Instance.currentData.materials;

    public override void RefreshItems()
        => CurrentItems = Source.OfType<IViewable>().ToList();

    public override void OnSlotSelected(int index)
    {
        MaterialItem item = Source[index];
        if (item.action.IsValid(item))
        {
            item.action.Execute(item);
            Source.RemoveAt(index);
        }
    }

    // materials가 일반 List라 가정 → 변경 알림 없음(닫을 때 전체 dirty로 커버됨).
    // 만약 materials도 ReactiveCollection<MaterialItem>이면 아래로 교체:
    //   private ReactiveCollection<MaterialItem> Source => GameManager.Instance.currentData.materials;
    //   public override IObservable<Unit> ObserveChanged() => ObserveAnyChange(Source);
    public override IObservable<Unit> ObserveChanged() => Observable.Empty<Unit>();
}