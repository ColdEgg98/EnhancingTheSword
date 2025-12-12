using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestGoStartScene : MonoBehaviour
{
    private Button btn;
    void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
    }
}
