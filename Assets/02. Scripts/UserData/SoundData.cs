using UnityEngine;

[System.Serializable]
public class SoundData {
    public string soundName;
    public SoundType type;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0f, .2f)] public float pitchVariance = 0f;
}

public enum SoundType
{
    BGM,
    SFX,
    ENV
}