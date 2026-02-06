using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("등록 목록")]
    [SerializeField] private SoundLibrary soundList; // 인스펙터에서 등록

    private Dictionary<string, SoundData> soundDict = new();

    [Header("오디오 플레이어")]
    [SerializeField] private AudioSource bgmPlayer;
    [SerializeField] private AudioSource sfxPlayer;
    [SerializeField] private AudioSource envPlayer;

    // Fade
    private float fadeTime;

    private void Awake()
    {
        fadeTime = 1f;
    }

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
        {
            // 1. 기본 피치(1.0)에서 설정된 변동폭만큼 랜덤하게 더하거나 뺌
            // 예: variance가 0.1이면 -> 0.9 ~ 1.1 사이의 값 생성
            float randomPitch = 1f + Random.Range(-data.pitchVariance, data.pitchVariance);

            // 2. 오디오 소스의 피치를 일시적으로 변경
            sfxPlayer.pitch = randomPitch;

            // 3. 재생 (변경된 피치로 발사됨)
            sfxPlayer.PlayOneShot(data.clip, data.volume);

            // 4. [중요] 다음 효과음을 위해 피치를 다시 정상(1.0)으로 복구
            sfxPlayer.pitch = 1f;
        }
    }

    public void PlayBGM(string name)
    {
        if (TryGetClip(name, SoundType.BGM, out SoundData data))
        {
            // 같은 곡이 재생 중이면 무시
            if (bgmPlayer.clip == data.clip && bgmPlayer.isPlaying)
            {
                // 방어 코드
                bgmPlayer.DOFade(data.volume, fadeTime);
                return;
            }

            // 페이드 중이었다면 없애기
            bgmPlayer.DOKill();

            // 음악 전환
            if (bgmPlayer.isPlaying && bgmPlayer.volume > 0)
            {
                // 음악 페이드 아웃
                bgmPlayer.DOFade(0f, fadeTime * .5f).OnComplete(() =>
                {
                    StartNewBGM(data, fadeTime * .5f);
                });
            }
            // 진행 중 음악이 없으면 바로 시작
            else
            {
                bgmPlayer.volume = 0f;
                StartNewBGM(data, fadeTime);
            }

            Debug.Log($"[PlayBGM] 제목 : {bgmPlayer.clip.name}, 볼륨 :{bgmPlayer.volume}");
        }
    }

    private void StartNewBGM(SoundData data, float fati)
    {
        bgmPlayer.clip = data.clip;
        bgmPlayer.Play();

        bgmPlayer.DOFade(data.volume, fati).SetEase(Ease.Linear);
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

    public void StopBGM()
    {
        bgmPlayer.DOKill();
        bgmPlayer.DOFade(0f, fadeTime * 2f).OnComplete(() =>
        {
            bgmPlayer.Stop();
        });
    }
    public void StopENV() => envPlayer.Stop();
    public void StopAllSound()
    {
        StopBGM();
        envPlayer.Stop();
        sfxPlayer.Stop();
    }
}
