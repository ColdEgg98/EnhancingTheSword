using UnityEngine;

/// <summary>
/// 골드, 아이템, 무기 등 획득시 호출되어 재화 수급 및 업적 감지 기능을 관리합니다.
/// </summary>
public class UserDataManager
{
    public int triggerValue;

    public void GetGold(int value)
    {
        GameManager.Instance.currentData.previewData.goldRef += value;
    }

    public void GetWeapon(Weapon newWeapon)
    {
        GameManager.Instance.currentData.myWeapons.Add(newWeapon);
    }

    public void GetWeapon(int ID)
    {
        if (!GameManager.Instance.allOfWeaponDictionary.ContainsKey(ID))
        {
            Debug.LogError($"확인되지 않은 무기 ID : {ID}");
            return;
        }

        Weapon newWeapon = GameManager.Instance.allOfWeaponDictionary[ID];
        GameManager.Instance.currentData.myWeapons.Add(newWeapon);
        Debug.Log($"무기 추가됨 : {newWeapon.addressID}");
    }
}

public class AchivementData
{
    AchivementData(UserData data)
    {
        this.iD = data.myWeapons[GameManager.Instance.selectWeaponIndex].addressID;
    }
    public string iD;
    public string Title;
    public string contnet;
    public Reward reward;
    public RewardType triggerType;
}

public class Reward
{
    public void ProcessReward(RewardType type, int index = 0, int value = 0)
    {
        switch (type)
        {
            case RewardType.Gold:
                GameManager.Instance.userDataManager.GetGold(value);
                break;
            case RewardType.Weapon:
                GameManager.Instance.userDataManager.GetWeapon(index);
                break;
            default:
                break;
        }
    }
}

public enum RewardType
{
    Gold,
    Weapon,
    Item,
    End
}