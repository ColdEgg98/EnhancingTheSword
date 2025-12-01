using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnhanceSword : MonoBehaviour
{
    // 모루 컴포넌트로 사용
    private Button button;
    private List<Weapon> myWeapons;
    private Weapon selectedWeapon;
    private int selectIndex;

    void Awake()
    {
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(RunEnhancing);
        myWeapons = GameManager.Instance.currentData.myWeapons;
        selectIndex = GameManager.Instance.selectWeaponIndex;
    }

    void RunEnhancing()
    {
        Debug.Log("RunEnhancing");
    }
}
