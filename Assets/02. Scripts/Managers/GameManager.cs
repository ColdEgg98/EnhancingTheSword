using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
    //public WarManager warManager;
    public AdManager adManager;

    // Dictionaries
    public Dictionary<int, Weapon> allOfWeaponDictionary;
    public Dictionary<string, Achievement> allOfAchivementDictionary;
    public Dictionary<ConditionType, List<Achievement>> AchieveByCondition;
    public OrderedDictionary allOfItemsDictionary;

    // Current Datas
    public int activeSaveSlotNum;
    public UserData currentData;
    public ReactiveProperty<long> gold;
    public IntReactiveProperty shippingSlot;

    // Current Weapon
    public ReactiveProperty<Weapon> currentWeapon;
    public ReactiveProperty<int> selectWeaponIndex;
    public BoolReactiveProperty isFocusOn;

    protected override void Awake()
    {
        base.Awake();

        uiManager = GetComponent<UIManager>();
        soundManager = GetComponent<SoundManager>();
        adManager = GetComponent<AdManager>();
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
        // 메인 씬 넘어갈 때 호출
        //warManager = GetComponent<WarManager>();
        //Instance.warManager.SetCurrentStage(currentData.stage);
        //Instance.warManager.LoadSlotData(currentData.deliverySlots);
        //Instance.uiManager.SetSlotImage(currentData.deliverySlots);
    }

    void Start()
    {
        Debug.Log("✅ [GameManager] : Start");
    }

    // 여기서부터 핫키 모음
    #region HotKeys

    public void GetGold(long value)
    {
        userDataManager.GetGold(value);
    }

    //public List<DeliverySlot> GetDeliverList() => warManager.GetSlots();

    //public void IncShippingSlot()
    //{
    //    userDataManager.IncShippingSlot();
    //}

    //public bool IsVaildDeliverWeapon(int weaponIndex, int slotIndex)
    //{
    //    return warManager.IsVaildDeliverWeapon(weaponIndex, slotIndex);
    //}

    //public void DeliverWeapon(int weaponIndex, int slotIndex)
    //{
    //    warManager.DeliverWeapon(weaponIndex, slotIndex);
    //}


    //public bool TryDeliverWeapon(int weaponIndex, int slotIndex)
    //{
    //    return warManager.TryDeliverWeapon(weaponIndex, slotIndex);
    //}

    public List<IViewable> GetViewableMyWeapons()
    {
        return GetMyWeapons().OfType<IViewable>().ToList();
    }
    
    public List<Weapon> GetMyWeapons()
    {
        return currentData.myWeapons;
    }

    public void ShowToast(string str)
    {
        uiManager.UIFactory.ShowToast(str);
    }

    public void ShowNotice(string str, Color c)
    {
        uiManager.UIFactory.ShowNotice(str, c);
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