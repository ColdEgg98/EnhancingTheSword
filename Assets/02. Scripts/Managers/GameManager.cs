using System.Collections.Generic;
using System.Collections.Specialized;
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
    public AAResourceManager aAResourceManager;
    public WarManager warManager;

    // Dictionaries
    public Dictionary<int, Weapon> allOfWeaponDictionary;
    public Dictionary<string, Achievement> allOfAchivementDictionary;
    public Dictionary<ConditionType, List<Achievement>> AchieveByCondition;
    public OrderedDictionary allOfItemsDictionary;

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
        aAResourceManager = new();

        allOfWeaponDictionary = new();
        allOfAchivementDictionary = new();
        AchieveByCondition = new();
        allOfItemsDictionary = new();

        selectWeaponIndex.Value = 0;
        currentWeapon = new();
        isFocusOn.Value = false;
    }

    public void InitGameManager()
    {
        warManager = GetComponent<WarManager>();
    }

    void Start()
    {
        Debug.Log("✅ [GameManager] : Start");
    }

    // 여기서부터 핫키 모음
    #region HotKeys
    public bool TryDeliverWeapon(int weaponIndex)
    {
        return warManager.TryDeliverWeapon(weaponIndex);
    }
    public List<Weapon> GetMyWeapons()
    {
        return currentData.myWeapons;
    }

    public void ShowToast(string str)
    {
        uiManager.UIFactory.ShowToast(str);
    }

    public void ShowNotice(string str)
    {
        uiManager.UIFactory.ShowNotice(str, Color.white);
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

    public void ModifyTipText(string s)
    {
        uiManager.ModifyTipText(s);
    }

    public void TipTextAppend(string s)
    {
        uiManager.TipTextAppend(s);
    }
    #endregion
}