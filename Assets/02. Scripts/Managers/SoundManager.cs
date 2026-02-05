using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    [Header("등록 목록")]
    [SerializeField] private List<SoundData> soundList;

    private Dictionary<string, SoundData> soundDict = new();

    [Header("오디오 플레이어")]
    [SerializeField] private AudioSource bgmPlayer;
    [SerializeField] private AudioSource sfxPlayer;
    [SerializeField] private AudioSource envPlayer;

    public void Init()
    {
        
    }
}
