using System.Collections.Generic;
using UnityEngine;

// 업적 체크 및 보상 프로세스 가동
// 멀티맵 구성의 AchieveByCondition 딕셔너리 사용
// 체크 항목은 게임 시작, 강화, 강화를 통한 무기 20단계 도달, 강화 실패 등

public class AchievementManager
{
    private Weapon preWeapon;

    public void Init()
    {
        preWeapon = GameManager.Instance.currentWeapon.Value;
    }

    public void CheckAchivement(ConditionType cType, long value)
    {
        Debug.Log($"CheckAchievement Run: {cType}, Value: {value}");
        if (GameManager.Instance.AchieveByCondition.TryGetValue(cType, out List<Achievement> targetList))
        {
            foreach (Achievement a in targetList)
            {
                if (GameManager.Instance.currentData.myAchievementRefs.Contains(a.AchivementID))
                    continue;

                if (a.ConditionValue <= value)
                {
                    ProcessReward(a);
                    GameManager.Instance.currentData.myAchievementRefs.Add(a.AchivementID);
                }
            }
        }
    }

    public void ProcessReward(Achievement a)
    {
        RewardType type = a.RewardType;
        float value = a.Value;

        GameManager.Instance.uiManager.UIFactory.ShowNotice($"{a.Name} 업적을 달성했습니다.", Color.white);

        switch (type)
        {
            case RewardType.Gold:
                GameManager.Instance.userDataManager.GetGold((long)value);
                GameManager.Instance.uiManager.UIFactory.ShowToast($"업적 보상 : {value} 골드");
                break;
            case RewardType.Weapon:
                GameManager.Instance.userDataManager.GetWeapon((int)value);
                Weapon newWeapon = GameManager.Instance.allOfWeaponDictionary[(int)value];
                GameManager.Instance.uiManager.UIFactory.ShowToast($"업적 보상 : {newWeapon.name}");
                break;
            case RewardType.ProbabilityBonus:
                GameManager.Instance.currentData.chanceBonus += value;
                GameManager.Instance.uiManager.UIFactory.ShowToast($"업적 보상 : {value}%p 강화 확률 상승");
                break;
            default:
                Debug.LogError("예외가 발생했습니다.");
                break;
        }
    }

    // SaveSlot에서 호출
    public async Awaitable GameStartAchieved()
    {
        await Awaitable.WaitForSecondsAsync(2.0f);
        GameManager.Instance.achievementManager.CheckAchivement(ConditionType.GameStart, 0);
    }
}
