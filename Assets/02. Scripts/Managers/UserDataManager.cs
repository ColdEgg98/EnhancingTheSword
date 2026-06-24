using UniRx;
using UnityEngine;

/// <summary>
/// 골드, 아이템, 무기 등 획득시 호출되어 재화 수급 및 업적 감지 기능을 관리합니다.
/// </summary>
public class UserDataManager
{
    public ReactiveProperty<int> featureCode = new(0);

    public void GetGold(long value)
    {
        if (value == 0) return;

        string message = StrUtiity.ToWonFormat(value);
        float Bonus;
        if (value > 0)
        {
            message += "를 획득했습니다.";

            // 추가 골드 계산 + 출력 메세지 수정
            if (GameManager.Instance.currentData.addtionalGold > 0)
            {
                Bonus = value / GameManager.Instance.currentData.addtionalGold;
                message += $"\n추가 골드 ({StrUtiity.ToWonFormat((long)Bonus)})";
                value += (long)Bonus;
            }
        }

        else if (value < 0)
        {
            message += "를 사용했습니다.";
        }

        // 골드 획득
        GameManager.Instance.gold.Value += value;

        // UI 표시
        GameManager.Instance.uiManager.UIFactory.ShowToast(message);

        // 업적 체크
        GameManager.Instance.currentData.totalGold += value;
        GameManager.Instance.achievementManager.CheckAchievement(ConditionType.TotalGold, GameManager.Instance.currentData.totalGold);
        GameManager.Instance.achievementManager.CheckAchievement(ConditionType.GoldAmount, GameManager.Instance.gold.Value);
    }

    public void GetGold(string value)
    {
        if (long.TryParse(value, out long amount))
        {
            GetGold(amount);
        }
        else
            Debug.LogError("❌ GetGold 과정 중 타입 변환에 실패했습니다.");
    }

    public void GetWeapon(int ID)
    {
        if (!GameManager.Instance.allOfWeaponDictionary.ContainsKey(ID))
        {
            Debug.LogError($"❌ 확인되지 않은 무기 ID : {ID}");
            return;
        }

        Weapon newWeapon = GameManager.Instance.allOfWeaponDictionary[ID];
        GameManager.Instance.currentData.myWeapons.Add(newWeapon);
        GameManager.Instance.ShowToast($"{StrUtiity.AttachJoSa(newWeapon.WeaponName)} 획득했습니다.");
        Debug.Log($"✅ 무기 추가됨 : {newWeapon.AddressID}");
    }

    public void GetWeapon(string strID)
    {
        if (!int.TryParse(strID, out int ID))
            Debug.LogWarning("❌ GetWeapon 과정 중 변환 실패");

        GetWeapon(ID);
    }

    public void GetItem(string id)
    {
        if (!GameManager.Instance.allOfItemsDictionary.Contains(id))
        {
            Debug.LogError($"❌ 확인되지 않은 아이템 ID : {id}");
            return;
        }

        MaterialItem newItem = (MaterialItem)GameManager.Instance.allOfItemsDictionary[id];
        GameManager.Instance.currentData.materials.Add(newItem);
        GameManager.Instance.ShowToast($"{StrUtiity.AttachJoSa(newItem.ItemName)} 획득했습니다.");
        Debug.Log($"✅ 아이템 추가됨 : {newItem.AddressID}");
    }

    public void GetItem(float id)
    {
        if (GameManager.Instance.allOfItemsDictionary[id] == null)
        {
            Debug.LogError($"❌ 확인되지 않은 아이템 ID : {id}");
            return;
        }

        MaterialItem newItem = (MaterialItem)GameManager.Instance.allOfItemsDictionary[id];
        GameManager.Instance.currentData.materials.Add(newItem);
        GameManager.Instance.ShowToast($"{StrUtiity.AttachJoSa(newItem.ItemName)} 획득했습니다.");
        Debug.Log($"✅ 아이템 추가됨 : {newItem.AddressID}");
    }

    public void BuyItem(string id, long price)
    {
        if (!GameManager.Instance.allOfItemsDictionary.Contains(id))
        {
            Debug.LogError($"❌ 확인되지 않은 아이템 ID : {id}");
            return;
        }

        MaterialItem newItem = (MaterialItem)GameManager.Instance.allOfItemsDictionary[id];
        GameManager.Instance.currentData.materials.Add(newItem);
        GameManager.Instance.gold.Value -= price;
        GameManager.Instance.ShowToast($"{StrUtiity.ToWonFormat(price)}를 지불하고,\n{StrUtiity.AttachJoSa(newItem.ItemName)} 획득했습니다.");
        Debug.Log($"✅ 아이템 추가됨 : {newItem.AddressID}");
    }

    public void IncShippingSlot()
    {
        GameManager.Instance.shippingSlot.Value++;
    }
}
