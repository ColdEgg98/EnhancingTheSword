using Cysharp.Threading.Tasks;
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

    public async void CheckAchievement(ConditionType cType, long value)
    {
        Debug.Log($"CheckAchievement Run: {cType}, Value: {value}");
        if (GameManager.Instance.AchieveByCondition.TryGetValue(cType, out List<Achievement> targetList))
        {
            foreach (Achievement a in targetList)
            {
                if (GameManager.Instance.currentData.myAchievementRefs.Contains(a.AchivementID))
                {
                    continue;
                }

                if (a.ConditionValue <= value && a.RewardType != RewardType.UnlockFeature)
                {
                    ProcessReward(a);
                    GameManager.Instance.currentData.myAchievementRefs.Add(a.AchivementID);
                    await GameManager.Instance.uiManager.UIFactory
                        .ShowAchievement(a.GetTextData(), EUIRole.MainImage, a.AchivementID);
                }
                else if (a.RewardType == RewardType.UnlockFeature)
                {
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
