using System.Collections.Generic;
using UniRx;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;

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
        uiManager = GetComponent<UIManager>();
        userDataManager = new();
        achievementManager = new();
        redSquareManager = new();

        allOfWeaponDictionary = new();
        allOfAchivementDictionary = new();
        AchieveByCondition = new();

        selectWeaponIndex.Value = 0;
        currentWeapon.Value = new();

        weaponXlsxFileName = "WeaponsData.xlsx";
        achivementXlsxFileName = "AchivementsData.xlsx";

        Application.targetFrameRate = 120;

        //Screen.SetResolution(1920, 1080, true);
    }

    async Awaitable Start()
    {
        await SaveEssentialDictionarys();
    }
    
// async Awaitable 메서드 (리턴값이 없을 때 Task 대신 사용)
    private async Awaitable SaveEssentialDictionarys()
    {
        // 1. 무기 데이터 로드 (값을 직접 리턴받음!)
        // 병렬 처리가 필요하면 여기서 Task.WhenAll 같은 패턴을 쓸 수도 있지만, 
        // 초기화 순서가 중요하다면 순차 실행이 안전합니다.
        
        var weaponList = await LoadTDatasAsync<Weapon>(weaponXlsxFileName);
        
        if (weaponList != null)
        {
            foreach (Weapon w in weaponList)
            {
                // 중복 키 체크 (안전장치)
                if (!allOfWeaponDictionary.ContainsKey(w.index))
                    allOfWeaponDictionary.Add(w.index, w);
            }
            Debug.Log($"✅ 무기 데이터 세팅 완료: {allOfWeaponDictionary.Count}개");
        }

        // 2. 업적 데이터 로드
        var achivementList = await LoadTDatasAsync<Achievement>(achivementXlsxFileName);
        
        if (achivementList != null)
        {
            foreach (Achievement a in achivementList)
            {
                if (!allOfAchivementDictionary.ContainsKey(a.AchivementID))
                    allOfAchivementDictionary.Add(a.AchivementID, a);

                if (!AchieveByCondition.ContainsKey(a.ConditionType))
                    AchieveByCondition[a.ConditionType] = new List<Achievement>();

                AchieveByCondition[a.ConditionType].Add(a);
            }
            Debug.Log($"✅ 업적 데이터 세팅 완료: {allOfAchivementDictionary.Count}개");
        }
    }

    // 제네릭 비동기 로드 함수
    // 리턴 타입이 Awaitable<T>가 됩니다.
    public async Awaitable<List<T>> LoadTDatasAsync<T>(string xlsxFileName) where T : class, new()
    {
        string path = Path.Combine(Application.streamingAssetsPath, xlsxFileName);
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS
            path = "file://" + path;
#endif

        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {
            // Unity 6.0 방식: AsyncOperation을 Awaitable로 변환하여 대기
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"❌ 파일 다운로드 실패 ({xlsxFileName}): {www.error}");
                return null; // 혹은 빈 리스트 반환
            }

            // 다운로드 성공, 데이터 파싱
            // 파싱 같은 무거운 작업은 메인 스레드를 잠깐 멈출 수 있으므로, 
            // 데이터가 아주 크다면 여기서 Background Thread로 넘기는 기술을 쓸 수도 있습니다.
            // (WebGL은 스레드 제약이 있으므로 여기선 메인 스레드에서 처리)
            try
            {
                byte[] data = www.downloadHandler.data;
                using (MemoryStream stream = new MemoryStream(data))
                {
                    // XlsxDataReader는 동기 함수지만, 여기서 호출하면 됩니다.
                    return XlsxDataReader<T>.MapFromExcel(stream);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ 엑셀 파싱 에러 ({xlsxFileName}): {e.Message}");
                return null;
            }
        }
    }
}