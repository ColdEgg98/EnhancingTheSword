using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public SaveDataManager saveDataManager;
    public UserDataManager userDataManager;

    public Dictionary<int, Weapon> allOfWeaponDictionary;
    public Dictionary<string, Achivement> allOfAchivementDictionary;
    private string weaponXlsxFileName;
    private string achivementXlsxFileName;

    public int activeSaveSlotNum;
    public UserData currentData;
    public ReactiveProperty<long> gold;

    public ReactiveProperty<Weapon> currentWeapon;
    public ReactiveProperty<int> selectWeaponIndex;

    protected override void Awake()
    {
        base.Awake();

        saveDataManager = GetComponent<SaveDataManager>();
        userDataManager = new UserDataManager();

        allOfWeaponDictionary = new ();
        allOfAchivementDictionary = new();

        selectWeaponIndex.Value = 0;
        currentWeapon.Value = new();

        weaponXlsxFileName = "WeaponsData.xlsx";
        achivementXlsxFileName = "AchivementsData.xlsx";
    }

    void Start()
    {
        SaveEssentialDictionarys();
    }
    
    private void SaveEssentialDictionarys()
    {
        List<Weapon> weaponList = SaveAllTDatas<Weapon>(weaponXlsxFileName);
        foreach(Weapon w in weaponList)
        {
            allOfWeaponDictionary.Add(w.index, w);
        }

        List<Achivement> achivementList = SaveAllTDatas<Achivement>(achivementXlsxFileName);
        foreach(Achivement a in achivementList)
        {
            allOfAchivementDictionary.Add(a.achivementID, a);
        }
    }

    public List<T> SaveAllTDatas<T>(string xlsxFileName) where T : class, new()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, $"{xlsxFileName}");
        List<T> TList = XlsxDataReader<T>.MapFromExcel(path);
        if (TList == null)
        {
            Debug.LogError($"{xlsxFileName}을 찾을 수 없습니다.");
            return default(List<T>);
        }
        return TList;
    }
}
