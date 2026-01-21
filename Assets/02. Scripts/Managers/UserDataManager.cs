using UnityEngine;

/// <summary>
/// 골드, 아이템, 무기 등 획득시 호출되어 재화 수급 및 업적 감지 기능을 관리합니다.
/// </summary>
public class UserDataManager
{    
    public void GetGold(long value)
    {
        GameManager.Instance.gold.Value += value;
        GameManager.Instance.currentData.totalGold += value;

        // 업적 체크
        GameManager.Instance.achievementManager.CheckAchivement(ConditionType.TotalGold, GameManager.Instance.currentData.enhanceCount++);
    }

    public void GetGold(string value)
    {
        if (long.TryParse(value, out long amount))
        {
            GetGold(amount);
        }
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

}
