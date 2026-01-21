using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public SaveDataManager saveDataManager;
    public UserDataManager userDataManager;
    public UIManager uiManager;
    public AchievementManager achievementManager;

    public Dictionary<int, Weapon> allOfWeaponDictionary;
    public Dictionary<string, Achievement> allOfAchivementDictionary;
    public Dictionary<ConditionType, List<Achievement>> AchieveByCondition;
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
        uiManager = GetComponent<UIManager>();
        achievementManager = new AchievementManager();

        allOfWeaponDictionary = new();
        allOfAchivementDictionary = new();
        AchieveByCondition = new();

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
        // 무기 xlsx 파일 데이터 저장
        List<Weapon> weaponList = SaveAllTDatas<Weapon>(weaponXlsxFileName);
        foreach (Weapon w in weaponList)
        {
            allOfWeaponDictionary.Add(w.index, w);
        }

        // 업적 xlsx 파일 데이터 저장
        List<Achievement> achivementList = SaveAllTDatas<Achievement>(achivementXlsxFileName);
        foreach (Achievement a in achivementList)
        {
            allOfAchivementDictionary.Add(a.AchivementID, a);

            // 딕셔너리 다중맵
            if (!AchieveByCondition.ContainsKey(a.ConditionType))
                AchieveByCondition[a.ConditionType] = new List<Achievement>();

            AchieveByCondition[a.ConditionType].Add(a);
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
