using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public SaveDataManager saveDataManager;
    public UserDataManager userDataManager;

    public Dictionary<string, Weapon> allOfWeaponDictionary;

    public int activeSaveSlotNum;
    public UserData currentData;

    public int selectWeaponIndex;

    protected override void Awake()
    {
        base.Awake();

        saveDataManager = GetComponent<SaveDataManager>();
    }

    void Start()
    {
        //SaveAllWeaponsData();
    }

    void SaveAllWeaponsData()
    {
        // 무기 데이터를 읽어서 allOf~에 집어넣기
        string weaponPath = System.IO.Path.Combine(Application.streamingAssetsPath, "WeaponDatas.xlsx");
        if (weaponPath == null)
            Debug.LogError("무기 데이터(엑셀 파일)를 찾을 수 없습니다.");
        List<Weapon> weaponList = XlsxDataReader<Weapon>.MapFromExcel(weaponPath);
        
        // 무기 데이터를 게임 매니저가 적재
        foreach(Weapon w in weaponList)
        {
            allOfWeaponDictionary.Add(w.id, w);
        }
    }
}
