using Cysharp.Threading.Tasks;

interface IMiniGame
{
    UniTask<MiniGameResult> Play();
}

public enum MiniGameGrade {Perfect, Good, Miss}

public class MiniGameResult
{
    public bool isPlayed;
    public MiniGameGrade grade;

    public float GetBonusChance() => grade switch
    {
        MiniGameGrade.Perfect => +20f,
        MiniGameGrade.Good => +10f,
        MiniGameGrade.Miss => 0f,
        _ => 0f
    };
}