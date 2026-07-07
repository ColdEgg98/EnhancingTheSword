using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 업적 체크 및 보상 프로세스 가동
// 멀티맵 구성의 AchieveByCondition 딕셔너리 사용
// 체크 항목은 게임 시작, 강화, 강화를 통한 무기 20단계 도달, 강화 실패 등

public class AchievementManager
{
    public async void CheckAchievement(ConditionType cType, long value)
    {
        Debug.Log($"CheckAchievement Run: {cType}, Value: {value}");

        // 업적 조건 신호가 오면 모든 업적 조건에서 같은 조건 신호만 추려서 리스트 생성
        if (GameManager.Instance.AchieveByCondition.TryGetValue(cType, out List<Achievement> targetList))
        {
            foreach (Achievement a in targetList)
            {
                // 이미 업적을 달성했으면 넘김
                if (GameManager.Instance.currentData.myAchievementRefs.Contains(a.AchivementID))
                {
                    continue;
                }

                // 기능 해금 업적
                if (a.ConditionValue == value && a.RewardType == RewardType.UnlockFeature)
                {
                    // 해금 코드를 변경해서 구독한 이벤트 발동으로 새 기능 해금
                    GameManager.Instance.SetFeautureCode((int)value);
                    GameManager.Instance.currentData.myAchievementRefs.Add(a.AchivementID);

                    // 업적 UI 표시
                    await GameManager.Instance.uiManager.UIFactory
                        .ShowAchievement(a.GetTextData(), EUIRole.MainImage, a.AchivementID);
                }

                // 낮을 업적 조건 확인
                else if (a.ConditionValue >= value && a.ConditionType == ConditionType.Below)
                {
                    ProcessRewardAndShow(a).Forget();
                }

                // 아이템 조건
                else if (a.ConditionType == ConditionType.HasItem)
                {
                    if (!HasItemCheck(a)) return;

                    ProcessRewardAndShow(a).Forget();
                }

                // 업적 코드 조건
                else if (a.ConditionType == ConditionType.AchieveCode && a.ConditionValue == value)
                {
                    ProcessRewardAndShow(a).Forget();
                }

                // 일반 업적 조건 확인
                else if (a.ConditionValue <= value && a.RewardType != RewardType.UnlockFeature)
                {
                    ProcessRewardAndShow(a).Forget();
                }
            }
        }
    }

    private async UniTaskVoid ProcessRewardAndShow(Achievement a)
    {
        ProcessReward(a);

        await GameManager.Instance.uiManager.UIFactory
            .ShowAchievement(a.GetTextData(), EUIRole.MainImage, a.AchivementID);
    }

    // 아이템 종류가 갯수만큼 있는지 비교
    private bool HasItemCheck(Achievement a)
    {
        List<Weapon> weapons = GameManager.Instance.GetMyWeapons();
        List<MaterialItem> materialItems = new List<MaterialItem>();
        int condition;

        for (int i = 0; i < a.ItemIndexes.Count; i++)
        {
            condition = 0;

            if (int.TryParse(a.ItemIndexes[i], out int index))
            {
                condition = weapons.Count(w => w.Index == index);
            }

            else
            {
                condition = materialItems.Count(m => m.ItemName == a.ItemIndexes[i]);
            }

            if (condition < a.HasItemAmount[i])
                return false;
        }

        return true;
    }

    public void ProcessReward(Achievement a)
    {
        GameManager.Instance.currentData.myAchievementRefs.Add(a.AchivementID);
        RewardType type = a.RewardType;
        float value = a.Value;

        switch (type)
        {
            case RewardType.Gold:
                GameManager.Instance.userDataManager.GetGold((long)value);
                GameManager.Instance.ShowToast($"업적 보상 : {StrUtiity.ToWonFormat((long)value)}");
                break;
            case RewardType.Weapon:
                GameManager.Instance.userDataManager.GetWeapon((int)value);
                Weapon newWeapon = GameManager.Instance.allOfWeaponDictionary[(int)value];
                GameManager.Instance.ShowToast($"업적 보상 : {StrUtiity.ColorText(newWeapon.WeaponName, "<color=#FFD700>")}");
                break;
            case RewardType.ProbabilityBonus:
                GameManager.Instance.currentData.chanceBonus += value;
                GameManager.Instance.ShowToast($"업적 보상 : 강화 확률 {StrUtiity.ColorText(value.ToString() + "%p")} 상승");
                break;
            case RewardType.AdditionalGold:
                GameManager.Instance.currentData.addtionalGold += value;
                GameManager.Instance.ShowToast($"업적 보상 : 골드 획득량 {StrUtiity.ColorText(value.ToString() + "%")} 증가");
                break;
            case RewardType.Item:
                GameManager.Instance.userDataManager.GetItem(a.ItemRewardAddress);
                break;
            case RewardType.EnhanceGold:
                GameManager.Instance.currentData.enhanceGold += value;
                GameManager.Instance.ShowToast($"업적 보상 : 강화 골드 {StrUtiity.ColorText(value.ToString() + "%")} 감소");
                break;
            default:
                Debug.LogError("예외가 발생했습니다.");
                break;
        }
    }

    // SaveSlot에서 호출
    public async UniTask GameStartAchieved()
    {
        await UniTask.WaitForSeconds(2.0f);
        GameManager.Instance.achievementManager.CheckAchievement(ConditionType.GameStart, 0);
    }
}
