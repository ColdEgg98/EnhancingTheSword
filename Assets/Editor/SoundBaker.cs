using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class SoundBaker
{


    // path에 BGM, SFX, ENV 붙여서 for문
    // for문 내용 : soundData(scriptable)로 저장

    [MenuItem("Tools/📚 UpdateLibrary")]
    public static void bakeSound()
    {
        // PathSetting
        string path = Path.Combine(Application.dataPath, "08. Sound/");
        string savePath = Path.Combine(path, "SoundDatas");
        string bgmPath = Path.Combine(path, "BGM");
        string sfxPath = Path.Combine(path, "SFX");
        string envPath = Path.Combine(path, "ENV");

        
        
    }
}
