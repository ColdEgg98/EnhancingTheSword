using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public SaveDataManager saveDataManager;
    public UserDataManager userDataManager;
    public UIManager uiManager;
    public AchievementManager achievementManager;
    public RedSquareManager redSquareManager;

    public Dictionary<int, Weapon> allOfWeaponDictionary;
    public Dictionary<string, Achievement> allOfAchivementDictionary;
    public Dictionary<ConditionType, List<Achievement>> AchieveByCondition;

    public int activeSaveSlotNum;
    public UserData currentData;
    public ReactiveProperty<long> gold;

    public ReactiveProperty<Weapon> currentWeapon;
    public ReactiveProperty<int> selectWeaponIndex;

    protected override void Awake()
    {
        base.Awake();

        uiManager = GetComponent<UIManager>();
        saveDataManager = new();
        userDataManager = new();
        achievementManager = new();
        redSquareManager = new();

        allOfWeaponDictionary = new();
        allOfAchivementDictionary = new();
        AchieveByCondition = new();

        selectWeaponIndex.Value = 0;
        currentWeapon.Value = new();
    }

    void Start()
    {
        Debug.Log("✅ [GameManager] : Start");
        saveDataManager.Init();
    }
}