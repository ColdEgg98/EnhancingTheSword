using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using System.Collections.Specialized;

public class LoadingHandler : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Slider slider;

    public Dictionary<int, Weapon> allOfWeaponDictionary = new();
    public Dictionary<string, Achievement> allOfAchivementDictionary = new();
    public OrderedDictionary allOfItemsDictionary = new();
    public Dictionary<ConditionType, List<Achievement>> AchieveByCondition = new();

    private string weaponXlsxFileName;
    private string achivementXlsxFileName;
    private string itemXlsxFileName;

    private int totalTasks;
    private int completedTasks;

    void Awake()
    {
        weaponXlsxFileName = "WeaponsData";
        achivementXlsxFileName = "AchivementsData";
        itemXlsxFileName = "ItemData";
        slider.value = 0;
        totalTasks = 0;
        completedTasks = 0;
    }

    void Start()
    {
        // Init
        GameManager.Instance.saveDataManager.Init();
        GameManager.Instance.soundManager.Init();

        SaveEssentialDictionarys().Forget();
    }

    public async UniTask SaveEssentialDictionarys()
    {
        // 시간 측정 시작
        Stopwatch sw = Stopwatch.StartNew();

        // 추가시 변경 해줄것
        totalTasks = 3;

        // 1. 데이터들 로드
        List<UniTask> tasks = new List<UniTask>
        {
            ProcessTasks($"Data/{weaponXlsxFileName}", LoadWeapons),
            ProcessTasks($"Data/{achivementXlsxFileName}", LoadAchievements),
            ProcessTasks($"Data/{itemXlsxFileName}", Loaditems)
        };

        await UniTask.WhenAll(tasks.ToArray());

        // 2. GameaManager 인스턴스에 복사
        SetGameManagerDatas();

        // 스탑워치 종료
        sw.Stop();
        Debug.Log($"⏱️ [SaveEssentialDictionarys] 로드 시간 : {sw.ElapsedMilliseconds}ms, ({sw.Elapsed.TotalSeconds}초)");

        // 씬 로드
        SceneManager.LoadScene(1);
    }

    private UniTask ProcessTasks(string path, Action<string> parseAction)
    {
        // Resources에서 텍스트 파일 로드
        TextAsset jsonFile = Resources.Load<TextAsset>(path);
        if (!jsonFile)
        {
            Debug.LogError($"❌ [LoadingHandler] : 파일을 찾을 수 없습니다 → {path}");
            return UniTask.CompletedTask;
        }

        // 파싱
        parseAction(jsonFile.text);

        // UI
        completedTasks++;
        float progress = (float)completedTasks / totalTasks;

        // 슬라이더 애니메이션
        SliderAnimation(progress).Forget();

        Debug.Log($"🔨 작업 완료 : ({completedTasks}/{totalTasks})");
        return UniTask.CompletedTask;
    }

    private async UniTask SliderAnimation(float progress)
    {
        Sequence seq = DOTween.Sequence();
        await seq.Append(slider.DOValue(progress, 0.05f));
        await seq.AsyncWaitForCompletion();
    }

    private void LoadWeapons(string json)
    {
        // Wrapper를 통해 리스트 복원
        var wrapper = JsonConvert.DeserializeObject<DataWrapper<Weapon>>(json);
        foreach (var w in wrapper.items)
        {
            if (!allOfWeaponDictionary.ContainsKey(w.Index))
                allOfWeaponDictionary.Add(w.Index, w);
        }
        Debug.Log($"⚔️ 무기 로드 완료: {wrapper.items.Count}개");
    }

    private void LoadAchievements(string json)
    {
        var wrapper = JsonConvert.DeserializeObject<DataWrapper<Achievement>>(json);
        foreach (var a in wrapper.items)
        {
            if (!allOfAchivementDictionary.ContainsKey(a.AchivementID))
                allOfAchivementDictionary.Add(a.AchivementID, a);

            if (!AchieveByCondition.ContainsKey(a.ConditionType))
                AchieveByCondition[a.ConditionType] = new List<Achievement>();

            AchieveByCondition[a.ConditionType].Add(a);
        }
        Debug.Log($"🏆 업적 로드 완료: {wrapper.items.Count}개");
    }
    private void Loaditems(string json)
    {
        var wrapper = JsonConvert.DeserializeObject<DataWrapper<MaterialItem>>(json);
        foreach (var item in wrapper.items)
        {
            allOfItemsDictionary.Add(item.AddressID, item);
        }
        Debug.Log($"🎁 아이템 로드 완료: {wrapper.items.Count}개");
    }

    private void SetGameManagerDatas()
    {
        GameManager.Instance.allOfAchivementDictionary = allOfAchivementDictionary;
        GameManager.Instance.allOfWeaponDictionary = allOfWeaponDictionary;
        GameManager.Instance.AchieveByCondition = AchieveByCondition;
        GameManager.Instance.allOfItemsDictionary = allOfItemsDictionary;
    }
}

