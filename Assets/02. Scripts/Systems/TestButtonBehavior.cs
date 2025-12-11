using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TestButtonBehavior : MonoBehaviour
{
    [SerializeField] GameObject saveSlotUI;
    [SerializeField] GameObject GetGoldUI;
    [SerializeField] GameObject GetWeaponUI;

    public void SaveTest()
    {
        GameManager.Instance.saveDataManager.StartSave();
    }

    public void LoadTest()
    {
        saveSlotUI.SetActive(true);
        
    }

    public void GetGoldTest()
    {
        GetGoldUI.SetActive(true);
    }

    public void GetWeaponTest()
    {
        GetWeaponUI.SetActive(true);
    }
}
