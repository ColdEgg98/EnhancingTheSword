using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public SaveDataManager saveDataManager;
    public UserDataManager userDataManager;

    public Dictionary<string, Weapon> allOfWeapons;

    public int activeSaveSlotNum;
    public UserData currentData;

    protected override void Awake()
    {
        base.Awake();

        saveDataManager = GetComponent<SaveDataManager>();
    }
}
