using System.Collections.Generic;
using UnityEditor.Build.Pipeline;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    [Header("등록 목록")]
    [SerializeField] private SoundLibrary soundList; // 인스펙터에서 등록

    private Dictionary<string, SoundData> soundDict = new();

    [Header("오디오 플레이어")]
    [SerializeField] private AudioSource bgmPlayer;
    [SerializeField] private AudioSource sfxPlayer;
    [SerializeField] private AudioSource envPlayer;

    public void Init()
    {
        // 오디오 소스 컴포넌트 자동 생성 (없을 경우)
        if (bgmPlayer == null) bgmPlayer = CreateAudioSource("BGM Player", true);
        if (sfxPlayer == null) sfxPlayer = CreateAudioSource("SFX Player", false);
        if (envPlayer == null) envPlayer = CreateAudioSource("ENV Player", true);

        if (soundList != null)
            soundDict = soundList.ToDictionary();
        else
            Debug.LogError("❗ [SoundManager] soundList를 등록하지 않았습니다.");
    }

    private AudioSource CreateAudioSource(string objName, bool isLoop)
    {
        GameObject go = new GameObject(objName);
        go.transform.SetParent(transform);
        AudioSource audio = go.AddComponent<AudioSource>();
        audio.loop = isLoop;
        audio.playOnAwake = false;
        return audio;
    }

    public void PlaySFX(string name)
    {
        if (TryGetClip(name, SoundType.SFX, out SoundData data))
            sfxPlayer.PlayOneShot(data.clip, data.volume);
    }

    public void PlayBGM(string name)
    {
        if (TryGetClip(name, SoundType.BGM, out SoundData data))
        {
            if (bgmPlayer.isPlaying && bgmPlayer.clip == data.clip) return;

            bgmPlayer.clip = data.clip;
            bgmPlayer.volume = data.volume;
            bgmPlayer.Play();
        }
    }

    public string GetCurrentBGMName()
    {
        return (bgmPlayer.clip != null) ? bgmPlayer.clip.name : string.Empty;
    }

    public void PlayENV(string name)
    {
        if (TryGetClip(name, SoundType.ENV, out SoundData data))
        {
            if (envPlayer.isPlaying && envPlayer.clip == data.clip) return;

            envPlayer.clip = data.clip;
            envPlayer.volume = data.volume;
            envPlayer.Play();
        }
    }

    private bool TryGetClip(string name, SoundType type, out SoundData data)
    {
        if (soundDict.TryGetValue(name, out data))
        {
            if (data.type != type)
            {
                Debug.LogWarning($"❗ [SoundManager : TryGetClip] 호출된 사운드 타입 불일치\n" +
                    $"요청 타입 : {type}, 실 타입 : {data.type}, 클립 이름 : {data.soundName}");
            }
            return true;
        }

        Debug.LogWarning($"❌ [SoundManager : TryGetClip] 사운드 찾을 수 없음 : {name}");
        data = null;
        return false;
    }

    public void StopBGM() => bgmPlayer.Stop();
    public void StopENV() => envPlayer.Stop();
    public void StopAllSound()
    {
        bgmPlayer.Stop();
        envPlayer.Stop();
        sfxPlayer.Stop();
    }
}
