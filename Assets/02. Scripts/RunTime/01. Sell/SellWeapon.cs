using System.Collections.Generic;
using UnityEngine;

public class SellWeapon : MonoBehaviour
{
    public Dictionary <int, Weapon> weaponsForSell;
    List<int> indexList;
    List<Weapon> myWeapons;

    void Awake()
    {
        weaponsForSell = new();
        indexList = new();
        myWeapons = GameManager.Instance.currentData.myWeapons;
    }

    public void SellButtonBehavior()
    {
        // 고등급일 때 재확인 추가 (함수로)
        long price = GameManager.Instance.currentWeapon.Value.price;
        GameManager.Instance.userDataManager.GetGold(price);
    }

    public void SellButtonBehavior(Dictionary <int, Weapon> data)
    {
        bool isHighGrade = false;
        long price = 0;

        foreach (var (key, value) in data)
        {
            // value 중에 고등급 있을 때 ID가 일정이상이면
            // isHighGrade = true
            price += value.price;
            indexList.Add(key);
        }
        if (!isHighGrade)
        {
            GameManager.Instance.userDataManager.GetGold(price);
            myWeapons.RemoveAll(weapon => indexList.Contains(weapon.index)); // 수정요
            Awake();
        }
        // else 재확인 함수 호출
    }
}
