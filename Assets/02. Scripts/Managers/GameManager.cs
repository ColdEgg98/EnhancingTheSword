using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    // Managers
    public SaveDataManager saveDataManager;
    public UserDataManager userDataManager;
    public UIManager uiManager;
    public AchievementManager achievementManager;
    public RedSquareManager redSquareManager;
    public SoundManager soundManager;

    // Dictionaries
    public Dictionary<int, Weapon> allOfWeaponDictionary;
    public Dictionary<string, Achievement> allOfAchivementDictionary;
    public Dictionary<ConditionType, List<Achievement>> AchieveByCondition;

    // Current Datas
    public int activeSaveSlotNum;
    public UserData currentData;
    public ReactiveProperty<long> gold;

    // Current Weapon
    public ReactiveProperty<Weapon> currentWeapon;
    public ReactiveProperty<int> selectWeaponIndex;
    public BoolReactiveProperty isFocusOn;

    protected override void Awake()
    {
        base.Awake();

        uiManager = GetComponent<UIManager>();
        soundManager = GetComponent<SoundManager>();
        saveDataManager = new();
        userDataManager = new();
        achievementManager = new();
        redSquareManager = new();

        allOfWeaponDictionary = new();
        allOfAchivementDictionary = new();
        AchieveByCondition = new();

        selectWeaponIndex.Value = 0;
        currentWeapon = new();
        isFocusOn.Value = false;
    }

    void Start()
    {
        Debug.Log("✅ [GameManager] : Start");
    }

    #region 핫키
    public void ShowToast(string str)
    {
        uiManager.UIFactory.ShowToast(str);
    }

    public void StopBGM()
    {
        soundManager.StopBGM();
    }

    public string GetCurrentBGMName()
    {
        Debug.Log($"[GetCurrentBGMName] : {soundManager.GetCurrentBGMName()}");
        return soundManager.GetCurrentBGMName();
    }

    public void PlayBGM(string name)
    {
        soundManager.PlayBGM(name);
    }

    public void SetFeautureCode(int code)
    {
        userDataManager.featureCode.Value = code;
    }
    #endregion
}