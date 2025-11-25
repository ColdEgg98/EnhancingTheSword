using System;
using System.IO;
using UnityEngine;

/// <summary>
/// 세이브하고 세이브파일을 로드하는 함수가 작성된 클래스입니다.
/// </summary>
public class SaveDataManager : MonoBehaviour
{
    string folderPath; // 게임.exe가 존재하는 폴더, 에디터에서는 Asset의 상위 폴더
    string indexPath; // MetaData 파일
    string path; // User Data{Num} 파일
    public WrapperPreviewData wrapperPreviewData;

    private void Awake()
    {
        PathSetting();
    }

    private void PathSetting()
    {
        int slotNum = GameManager.Instance.activeSaveSlotNum;

        // 실행 파일 혹은 프로젝트 루트 폴더에 세이브 경로를 확보합니다.
#if UNITY_STANDALONE || UNITY_EDITOR
        folderPath = Directory.GetParent(Application.dataPath).FullName;
        indexPath = Path.Combine(folderPath, "MetaData.json");
        path = Path.Combine(folderPath, $"User Data{slotNum}.json");
#else
        path = System.IO.Path.Combine(Application.persistentDataPath, "User Data.json");
#endif
        Debug.Log("PathSetting is Run");
    }

    #region Save Data
    /// <summary>
    /// 현재 슬롯에 저장합니다.
    /// </summary>
    public void StartSave()
    {
        PathSetting();

        UserData data = new()
        {
            previewData = GameManager.Instance.currentData.previewData,
            test = GameManager.Instance.currentData.test,
            // 무기 현황 저장
        };
        data.previewData.time = DateTime.Now.ToString("yyyy.MM.dd\ntt hh시 mm분");

        IndexDataSave();
        UserDataSave(data);
    }

    /// <summary>
    /// activeSaveSlotNum을 index 값으로 할당하고 세이브를 시작합니다
    /// </summary>
    /// <param name="index">'index' 번째 슬롯에 저장됩니다.</param>
    public void StartSave(int index)
    {
        GameManager.Instance.activeSaveSlotNum = index;
        StartSave();
    }

    /// <summary>세이브 슬롯 UI에 표시되는 데이터만 모은 인덱스 데이터를 저장</summary>
    private void IndexDataSave()
    {
        GameManager.Instance.saveDataManager.wrapperPreviewData.slots[GameManager.Instance.activeSaveSlotNum] = GameManager.Instance.currentData.previewData;
        string jsonStringIndex = JsonUtility.ToJson(GameManager.Instance.saveDataManager.wrapperPreviewData, true);
        File.WriteAllText(indexPath, jsonStringIndex);
        Debug.Log("PreviewData Saved");
    }

    /// <summary>본게임에 사용되는 User의 데이터 저장</summary>
    private void UserDataSave(UserData data)
    {
        string jsonString = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, jsonString);
    }
    #endregion

    #region Load Data
    /// <summary>GameManager Instance의 wrapper~에 MetaData 정보를, currentData에 User DataN을 읽어와 할당합니다.</summary>
    /// <param name="index">User DataN에서 N을 맡고 있습니다</param>
    public void StartLoad(int index)
    {
        PathSetting();

        //GameManager.Instance.currentData.previewData = PreviewDataLoad(index);
        GameManager.Instance.currentData = UserDataLoad(index);
    }

    /// <summary>UI에 표기되는 데이터 로드</summary>
    /// <returns>MetaData.json 파일에서 읽음</returns>
    public PreviewData PreviewDataLoad(int index)
    {
        if (File.Exists(indexPath))
        {
            string data = File.ReadAllText(indexPath);
            WrapperPreviewData WPD = JsonUtility.FromJson<WrapperPreviewData>(data);
            return WPD.slots[index];
        }
        else
        {
            Debug.Log("PreviewData Load failed");
            return new PreviewData();
        }
    }

    /// <summary>본 게임에 사용되는 데이터 로드</summary>
    /// <returns>User Data{슬롯 번호}.json 파일에서 읽음</returns>
    private UserData UserDataLoad(int index)
    {
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            return JsonUtility.FromJson<UserData>(data);
        }
        else if (File.Exists(indexPath))
            return new UserData(JsonUtility.FromJson<WrapperPreviewData>(File.ReadAllText(indexPath)).slots[index]);
        else
            return new UserData(new PreviewData());
    }
    #endregion

    /// <summary>UserDataN.json 파일의 경로를 반환합니다.</summary>
    public string GetPath()
    {
        return path;
    }

    /// <summary>MetaData.json 파일의 경로를 반환합니다.</summary>
    public string GetIndexPath()
    {
        return indexPath;
    }
}
