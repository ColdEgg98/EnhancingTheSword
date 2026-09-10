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
        MiniGameGrade.Perfect => 0.15f,
        MiniGameGrade.Good => 0.75f,
        MiniGameGrade.Miss => -0.05f,
        _ => 0f
    };
}