using System.Collections.Generic;
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

    /// <summary>
    /// 로드 시 presenter를 통해 view에게 그리도록 요청
    /// </summary>
    //public void SetSlotImage(List<DeliverySlot> deliveryItems)
    //{
    //    // 세이브 데이터에 전송 이력이 없으면 리턴
    //    if (deliveryItems == null) return;

    //    SlotPresenter presenter = FindAnyObjectByType<SlotPresenter>(FindObjectsInactive.Include);
    //    presenter.SetSlotBind();


    //    foreach (DeliverySlot item in deliveryItems)
    //    {
    //        presenter.SetSlot((item.SlotIndex, item.WeaponLevel), item);
    //    }
    //}

    public void ModifyTipText(string s)
    {
        if (!tipList.Contains(s))
            tipList.Add(s);
        else
            tipList.Remove(s);
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
