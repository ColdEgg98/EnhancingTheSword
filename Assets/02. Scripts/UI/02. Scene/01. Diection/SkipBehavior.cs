using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkipBehavior : MonoBehaviour
{
    private OpeningDirection opening;

    private void Awake()
    {
        opening = FindAnyObjectByType<OpeningDirection>();
    }

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => opening.SkipOpening());
    }
}
