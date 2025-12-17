using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

/// <summary>
/// 골드, 아이템, 무기 등 획득시 호출되어 재화 수급 및 업적 감지 기능을 관리합니다.
/// </summary>
public class UserDataManager
{    
    public UserDataManager()
    {
        // 골드같은거 구독해서 조건 검색
    }

    public void GetGold(int value)
    {
        GameManager.Instance.gold.Value += value;
    }

    public void GetGold(string value)
    {
        if (long.TryParse(value, out long amount))
            GameManager.Instance.gold.Value += amount;
        else
            Debug.LogError("GetGold 과정 중 타입 변환에 실패했습니다.");
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

    public void GetWeapon(string strID)
    {
        if (!int.TryParse(strID, out int ID))
            Debug.LogWarning("GetWeapon 과정 중 변환 실패");

        if (!GameManager.Instance.allOfWeaponDictionary.ContainsKey(ID))
        {
            Debug.LogError($"확인되지 않은 무기 ID : {ID}");
            return;
        }

        Weapon newWeapon = GameManager.Instance.allOfWeaponDictionary[ID];
        GameManager.Instance.currentData.myWeapons.Add(newWeapon);
        Debug.Log($"무기 추가됨 : {newWeapon.addressID}");
    }

    public void GetItem(int ID)
    {
        // 아이템 딕셔너리 검색
        // Toast를 UIManager에서 출력
    }

    public void ProcessReward(RewardType type, int value = 0)
    {
        switch (type)
        {
            case RewardType.Gold:
                GameManager.Instance.userDataManager.GetGold(value);
                break;
            case RewardType.Weapon:
                GameManager.Instance.userDataManager.GetWeapon(value);
                break;
            case RewardType.ProbabilityBonus:
                GameManager.Instance.currentData.chanceBonus += value;
                break;
            default:
                break;
        }
    }
}


[Serializable]
public class Achivement : IDisposable
{
    public string achivementID { get; set; }
    public string description { get; set; }
    public RewardType rewardType { get; set; }
    public float value { get; set; }
    // 조건 관련 변수 추가되어야 할듯
    public bool isAchive;
    private AsyncOperationHandle<Sprite> _handle;
    public async Task AchivementSpriteApply(Image targetImage)
    {
        if (_handle.IsValid())
            Addressables.Release(_handle);

        _handle = Addressables.LoadAssetAsync<Sprite>(achivementID);
        await _handle.Task;
        targetImage.sprite = _handle.Result;
    }

    // 사용하는 UI에서 Dispose를 잘해줘야함.
    // 업적 이미지가 내려갈 때 Dispose를 호출하게되거나
    // 업적 Monobehavior 상속된 핸들러 클래스를 만들어서 업적 UI에 붙이면될듯
    // 만약 이 솔루션으로 되면 dispose대신 ondestory 쓰면 되고
    public void Dispose()
    {
        if (_handle.IsValid())
            Addressables.Release(_handle);
        _handle = default;
    }
}

public enum RewardType
{
    Gold,
    Weapon,
    Item,
    ProbabilityBonus,
    End
}