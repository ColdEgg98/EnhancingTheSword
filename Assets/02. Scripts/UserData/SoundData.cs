using UnityEngine;

[System.Serializable]
public class SoundData : ScriptableObject {
    public string soundName;
    public SoundType type;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
}

public enum SoundType
{
    BGM,
    SFX,
    Environment
}