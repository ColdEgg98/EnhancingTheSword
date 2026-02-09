using UniRx;
using UnityEngine;

public class BGMHandler : MonoBehaviour
{
    private readonly string[] normalBgms = new string[]
    {
        "Creative",
        "Peaceful Stroll",
        "TechnoBeat"
    };

    private readonly string[] highLevelBgms = new string[]
    {
        "HighLevelDemon"
    };

    private int levelThreshold = 13;

    private void Start()
    {
        Sub();
    }

    private void Sub()
    {
        GameManager.Instance.currentWeapon
            .Subscribe(weapon =>
            {
                if (weapon != null)
                    LevelBGM(weapon.Index);
            })
            .AddTo(this);
    }

    private void LevelBGM(int level)
    {
        string bgmName;

        if (level >= levelThreshold)
        {
            int randomIndex = Random.Range(0, highLevelBgms.Length);
            bgmName = highLevelBgms[randomIndex];
        }
        else
        {
            if (IsPlayingNormalBGM()) return;

            int randomIndex = Random.Range(0, normalBgms.Length);
            bgmName = normalBgms[randomIndex];
        }

        GameManager.Instance.PlayBGM(bgmName);
    }

    private bool IsPlayingNormalBGM()
    {
        string current = GameManager.Instance.GetCurrentBGMName();
        foreach (var bgm in normalBgms)
        {
            if (current == bgm) return true;
        }
        return false;
    }
}
