using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SoundBaker : Editor
{
    // path에 BGM, SFX, ENV 붙여서 for문
    // for문 내용 : soundData(scriptable)로 저장

    private const string SOUND_ROOT_PATH = "Assets/08. Sound/";
    private const string LIBRARY_PATH = "Assets/08. Sound/SoundLibrary.asset";

    [MenuItem("Tools/📚 UpdateLibrary")]
    public static void bakeSound()
    {
        // 라이브러리 SO 생성 || 로드
        SoundLibrary instance = AssetDatabase.LoadAssetAtPath<SoundLibrary>(LIBRARY_PATH);
        if (instance == null)
        {
            instance = CreateInstance<SoundLibrary>();
            AssetDatabase.CreateAsset(instance, LIBRARY_PATH);
        }

        // 기존 리스트 백업
        Dictionary<string, SoundData> backupDict = instance.ToDictionary();

        instance.soundList.Clear();

        // 폴더 스캔
        string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] {SOUND_ROOT_PATH});

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);

            SoundType type = SoundType.SFX;
            if (assetPath.Contains("/BGM/")) type = SoundType.BGM;
            else if (assetPath.Contains("/ENV/")) type = SoundType.ENV;

            // 데이터 생성
            SoundData newData = new();
            newData.soundName = clip.name;
            newData.clip = clip;
            newData.type = type;

            // 백업 참조
            if (backupDict.ContainsKey(newData.soundName))
                newData.volume = backupDict[newData.soundName].volume;
            else
                newData.volume = 1f;

            instance.soundList.Add(newData);
        }

        // 저장 및 갱신
        EditorUtility.SetDirty(instance);
        AssetDatabase.SaveAssets();

        Debug.Log($"📚 Library 갱신 완료!\n등록된 클립 수 : {instance.soundList.Count}개");
    }
}
