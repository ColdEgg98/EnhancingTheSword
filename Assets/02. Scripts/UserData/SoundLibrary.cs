using System.Collections.Generic;
using UnityEngine;

// SoundBaker에서 변경될 항목
[CreateAssetMenu(fileName = "SoundLibrary", menuName = "SO/SoundLibrary", order = 0)]
public class SoundLibrary : ScriptableObject
{
    public List<SoundData> soundList = new();

    // 헬퍼 함수
    public Dictionary<string, SoundData> ToDictionary()
    {
        Dictionary<string, SoundData> dict = new();
        foreach (SoundData data in soundList)
        {
            if (dict.ContainsKey(data.soundName)) continue;
            dict.Add(data.soundName, data);
        }
        return dict;
    }
}
