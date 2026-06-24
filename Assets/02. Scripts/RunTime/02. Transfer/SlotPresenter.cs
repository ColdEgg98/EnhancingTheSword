//using Cysharp.Threading.Tasks;
//using System;
//using TMPro;
//using UniRx;
//using UnityEngine;
//using UnityEngine.UI;

//public class SlotPresenter : MonoBehaviour
//{
//    [SerializeField] private SlotGridView slotView;

//    [SerializeField] Transform ItemContents;
//    private SupplyItemButton[] supplyItemButtons;
//    private ReactiveCollection<WeaponSupplySlotButton> _slots;

//    void Awake()
//    {
//        //supplyItemButtons = ItemContents.GetComponentsInChildren<SupplyItemButton>(); // 20개
//        //SubForSupplyButtons();
//    }

//    private void Start()
//    {
//        //SetSlotBind();
//    }

//    public void SetSlotBind()
//    {
//        _slots = slotView.slots;
//    }

//    private void SubForSupplyButtons()
//    {
//        foreach (var b in supplyItemButtons)
//        {
//            AgreeCallSlot(b);
//        }
//    }

///// <summary>
///// 인벤토리에서 보급 무기가 선택됐을 때, 보급 슬롯의 이미지를 변경합니다.
///// </summary>
//    private void AgreeCallSlot(SupplyItemButton item)
//    {
//        item.onClickSlot
//            .Subscribe(e =>
//            {
//                SetSlot(e);
//            })
//            .AddTo(this)
//            .AddTo(item);
//    }

//    public void SetSlot((int slotNumber, int weaponIndex) e, DeliverySlot slotData = null)
//    {
//        // 슬롯 이미지 세팅
//        int targetIndex = e.slotNumber;
//        Image image = _slots[targetIndex].GetComponent<Image>();
//        image.color = Color.white;
//        image.raycastTarget = false;

//        Weapon tempWeapon;
//        float deliveryTime;
//        float influenceDuration;
//        bool PhaseFlag;

//        if (slotData != null)
//        {
//            // 세이브 데이터를 로드할 때 호출
//            tempWeapon = GameManager.Instance.allOfWeaponDictionary[slotData.WeaponLevel];
//            deliveryTime = slotData.DeliveryTimeRemaining;
//            influenceDuration = slotData.InfluenceTimeRemaining;
//            PhaseFlag = slotData.IsDelivered;
//        }
//        else
//        {
//            tempWeapon = GameManager.Instance.currentData.myWeapons[e.weaponIndex];
//            deliveryTime = tempWeapon.DeliveryTime;
//            influenceDuration = tempWeapon.InfluenceDuration;
//            PhaseFlag = false;
//        }

//        IViewable tempViewable = tempWeapon;

//        GameManager.Instance.aAResourceManager.
//            SetSpriteAsync(tempViewable, _slots[targetIndex].slotImage).Forget();

//        // 슬롯 시간 세팅
//        TextMeshProUGUI timeItem = _slots[targetIndex].GetComponentInChildren<TextMeshProUGUI>();
//        TimeCountSet(timeItem, deliveryTime, influenceDuration, PhaseFlag, () =>
//        {
//            // 시간 표시가 끝나면 이미지를 다시 비워줌
//            image.color = Color.clear;
//            image.raycastTarget = true;
//        });
//    }

//    /// <summary>
//    /// 보급 슬롯의 남은 시간 표기(출하 및 영향력 행사 시간)를 관리합니다.
//    /// </summary>
//    private void TimeCountSet(TextMeshProUGUI textItem, float deliveryTime, float influenceDuration, bool isInfluence, Action onCompleted)
//    {
//        float remainingTime;
//        bool isInfluencePhase = isInfluence; // 현재 영향력 단계인지 여부

//        // 초기 텍스트 설정
//        if (isInfluencePhase == false)
//        {
//            remainingTime = deliveryTime;
//            textItem.SetText("출하까지 : {0}", (int)remainingTime);
//        }
//        else
//        {
//            // 세이브로부터 주입 받을 시
//            remainingTime = influenceDuration;
//            textItem.SetText("무기 영향력 남은 시간 : {0}", (int)remainingTime);
//        }

//        // 1초마다 반복하는 타이머
//        Observable.Interval(TimeSpan.FromSeconds(1f))
//            // 종료 조건: 영향력 단계에서 시간이 0 이하가 되면 스트림 종료
//            .TakeWhile(_ => !(isInfluencePhase && remainingTime <= 0))
//            .Subscribe(_ =>
//            {
//                remainingTime--;

//                // 1단계(출하) 종료 및 2단계(영향력) 전환 체크
//                if (!isInfluencePhase && remainingTime <= 0)
//                {
//                    isInfluencePhase = true;
//                    remainingTime = influenceDuration;
//                    textItem.SetText("무기 영향력 남은 시간 : {0}", (int)remainingTime);
//                    return; // 페이즈 전환 시 이번 틱은 종료
//                }

//                // 현재 페이즈에 따른 텍스트 갱신
//                if (!isInfluencePhase)
//                {
//                    textItem.SetText("출하까지 : {0}", (int)remainingTime);
//                }
//                else
//                {
//                    // 영향력 시간 갱신 (0보다 클 때만)
//                    if (remainingTime > 0)
//                        textItem.SetText("무기 영향력 남은 시간 : {0}", (int)remainingTime);
//                    else
//                    {
//                        textItem.SetText(string.Empty); // 최종 종료 시 비움
//                        onCompleted?.Invoke(); // 람다식 실행
//                    }
//                }
//            })
//            .AddTo(textItem);
//    }
//}
