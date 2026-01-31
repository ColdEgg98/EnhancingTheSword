using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.Networking;
using System.Diagnostics;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using System.Threading.Tasks;
using DG.Tweening;
using ClosedXML.Excel;
using ClosedXML.Graphics;
using UnityEngine.SceneManagement;

public class LoadingHandler : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Slider slider;

    public static LoadOptions loadOptions { get; private set; }
    public Dictionary<int, Weapon> allOfWeaponDictionary = new();
    public Dictionary<string, Achievement> allOfAchivementDictionary = new();
    public Dictionary<ConditionType, List<Achievement>> AchieveByCondition = new();

    private string weaponXlsxFileName;
    private string achivementXlsxFileName;
    private string fontFileName;

    private int totalTasks;
    private int completedTasks;

    void Awake()
    {
        weaponXlsxFileName = "WeaponsData.xlsx";
        achivementXlsxFileName = "AchivementsData.xlsx";
        fontFileName = "FallBackFont.ttf";
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

        // 1. 폰트 데이터 우선 로드 및 대기
        var task = ProcessTasks(LoadFontAsync());
        await task;

        // 2. 그 외 데이터들 병렬로 로드
        List<Task> tasks = new List<Task>
        {
            ProcessTasks(LoadWeaponDatas()),
            ProcessTasks(LoadAchevementDatas())
        };
        totalTasks = tasks.Count + 1;

        await Task.WhenAll(tasks.ToArray());

        // 3. GameaManager 인스턴스에 복사
        SetGameManagerDatas();

        // 스탑워치 종료
        sw.Stop();
        Debug.Log($"⏱️ [SaveEssentialDictionarys] 로드 시간 : {sw.ElapsedMilliseconds}ms, ({sw.Elapsed.TotalSeconds}초)");

        // 씬 로드
        SceneManager.LoadScene(1);
    }

    private async Task ProcessTasks(Awaitable task)
    {
        await task;

        completedTasks++;
        float progress = (float)completedTasks / totalTasks;

        await slider.DOValue(progress, 0.15f).AsyncWaitForCompletion();

        Debug.Log($"✅ 작업 완료 : ({completedTasks}/{totalTasks})");
    }

    // 폰트 로드
    private async Awaitable LoadFontAsync()
    {
        string path = GetPath(fontFileName);

        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {
            await www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success) Debug.LogError($"❌ 폰트 다운로드 실패 : [{www.error}]");
            
            using (MemoryStream ms = new MemoryStream(www.downloadHandler.data))
            {
                loadOptions = new LoadOptions
                {
                    GraphicEngine = DefaultGraphicEngine.CreateOnlyWithFonts(ms)
                };
            }
        Debug.Log("✅ 폰트 설정 완료");
        }
    }

    private async Awaitable LoadAchevementDatas()
    {
        var list = await LoadTDatasAsync<Achievement>(achivementXlsxFileName);
        if (list != null)
        {
            foreach (Achievement a in list)
            {
                if (!allOfAchivementDictionary.ContainsKey(a.AchivementID))
                    allOfAchivementDictionary.Add(a.AchivementID, a);

                if (!AchieveByCondition.ContainsKey(a.ConditionType))
                    AchieveByCondition[a.ConditionType] = new List<Achievement>();

                AchieveByCondition[a.ConditionType].Add(a);
            }
        }
    }


    private async Awaitable LoadWeaponDatas()
    {
        var list = await LoadTDatasAsync<Weapon>(weaponXlsxFileName);
        if (list != null)
        {
            // 병렬 처리 중 딕셔너리 접근 시 스레드 충돌 가능성이 있으나,
            // Unity Awaitable은 메인 스레드 컨텍스트로 돌아오므로 lock이 필수는 아닙니다.
            foreach (Weapon w in list)
            {
                if (!allOfWeaponDictionary.ContainsKey(w.index))
                    allOfWeaponDictionary.Add(w.index, w);
            }
        }
    }

    // 공용 로더
    public async Awaitable<List<T>> LoadTDatasAsync<T>(string fileName) where T : class, new()
    {
        string path = GetPath(fileName);
        
        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"❌ 로드 실패 ({fileName}): {www.error}");
                return null;
            }

            try
            {
                byte[] data = www.downloadHandler.data;
                using (MemoryStream stream = new MemoryStream(data))
                {
                    var result = XlsxDataReader<T>.MapFromExcel(stream);
                    
                    return result;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ 파싱 에러 ({fileName}): {e.Message}");
                return null;
            }
        }
    }

    private string GetPath(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
        path = "file://" + path;
#elif UNITY_WEBGL
    // 윈도우 빌드 환경에서 생길 수 있는 역슬래시(\)를 슬래시(/)로 바꿔야 함
    // 브라우저는 역슬래시 경로를 인식하지 못함
    path = path.Replace("\\", "/");
#endif
        return path;
    }
    
    private void SetGameManagerDatas()
    {
        GameManager.Instance.allOfAchivementDictionary = allOfAchivementDictionary;
        GameManager.Instance.allOfWeaponDictionary = allOfWeaponDictionary;
        GameManager.Instance.AchieveByCondition = AchieveByCondition;
    }
}

