using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;
using Newtonsoft.Json;

public class LoadingHandler : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Slider slider;

    public Dictionary<int, Weapon> allOfWeaponDictionary = new();
    public Dictionary<string, Achievement> allOfAchivementDictionary = new();
    public Dictionary<ConditionType, List<Achievement>> AchieveByCondition = new();

    private string weaponXlsxFileName;
    private string achivementXlsxFileName;

    private int totalTasks;
    private int completedTasks;

    void Awake()
    {
        weaponXlsxFileName = "WeaponsData";
        achivementXlsxFileName = "AchivementsData";
        slider.value = 0;
        totalTasks = 0;
        completedTasks = 0;
    }

    async Awaitable Start()
    {
        await SaveEssentialDictionarys();
    }

    public async Awaitable SaveEssentialDictionarys()
    {
        // 시간 측정 시작
        Stopwatch sw = Stopwatch.StartNew();

        totalTasks = 2;

        // 1. 데이터들 병렬로 로드
        List<Task> tasks = new List<Task>
        {
            ProcessTasks($"Data/{weaponXlsxFileName}", LoadWeapons),
            ProcessTasks($"Data/{achivementXlsxFileName}", LoadAchievements)
        };

        await Task.WhenAll(tasks.ToArray());

        // 2. GameaManager 인스턴스에 복사
        SetGameManagerDatas();

        // 스탑워치 종료
        sw.Stop();
        Debug.Log($"⏱️ [SaveEssentialDictionarys] 로드 시간 : {sw.ElapsedMilliseconds}ms, ({sw.Elapsed.TotalSeconds}초)");

        // 씬 로드
        SceneManager.LoadScene(1);
    }

    private async Task ProcessTasks(string path, Action<string> parseAction)
    {
        // Resources에서 텍스트 파일 로드
        TextAsset jsonFile = Resources.Load<TextAsset>(path);
        if (!jsonFile)
        {
            Debug.LogError($"❌ [LoadingHandler] : 파일을 찾을 수 없습니다 → {path}");
            return;
        }

        // 파싱
        parseAction(jsonFile.text);

        // UI
        completedTasks++;
        float progress = (float)completedTasks / totalTasks;

        // 슬라이더 애니메이션
        _ = slider.DOValue(progress, 0.2f);

        Debug.Log($"🔨 작업 완료 : ({completedTasks}/{totalTasks})");
    }

    private void LoadWeapons(string json)
    {
        // Wrapper를 통해 리스트 복원
        var wrapper = JsonConvert.DeserializeObject<DataWrapper<Weapon>>(json);
        foreach (var w in wrapper.items)
        {
            if (!allOfWeaponDictionary.ContainsKey(w.index))
                allOfWeaponDictionary.Add(w.index, w);
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

    private void SetGameManagerDatas()
    {
        GameManager.Instance.allOfAchivementDictionary = allOfAchivementDictionary;
        GameManager.Instance.allOfWeaponDictionary = allOfWeaponDictionary;
        GameManager.Instance.AchieveByCondition = AchieveByCondition;
    }
}

