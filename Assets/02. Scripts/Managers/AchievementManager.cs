using Cysharp.Threading.Tasks;
using System.Collections.Generic;
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

                // 일반 업적 조건 확인
                if (a.ConditionValue <= value && a.RewardType != RewardType.UnlockFeature)
                {
                    GameManager.Instance.currentData.myAchievementRefs.Add(a.AchivementID);
                    ProcessReward(a);
                    
                    await GameManager.Instance.uiManager.UIFactory
                        .ShowAchievement(a.GetTextData(), EUIRole.MainImage, a.AchivementID);
                }

                // 낮을 업적 조건 확인
                else if (a.ConditionValue >= value && a.ConditionType == ConditionType.Below)
                {
                    GameManager.Instance.currentData.myAchievementRefs.Add(a.AchivementID);
                    ProcessReward(a);

                    await GameManager.Instance.uiManager.UIFactory
                        .ShowAchievement(a.GetTextData(), EUIRole.MainImage, a.AchivementID);
                }

                // 기능 해금 업적
                else if (a.RewardType == RewardType.UnlockFeature)
                {
                    await GameManager.Instance.uiManager.UIFactory
                        .ShowAchievement(a.GetTextData(), EUIRole.MainImage, a.AchivementID);
                    // 해금 코드를 변경해서 구독한 이벤트 발동으로 새 기능 해금
                    GameManager.Instance.SetFeautureCode((int)value);
                }
            }
        }
    }

    public void ProcessReward(Achievement a)
    {
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
                // 임시 : 중급 파괴 방지 물약만 받음
                GameManager.Instance.userDataManager.GetItem("MidGrade AD");
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
