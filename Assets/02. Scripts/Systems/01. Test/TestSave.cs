using UnityEngine;
using UnityEngine.UI;

public class TestSave : MonoBehaviour
{
    private Button btn;
    void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            GameManager.Instance.saveDataManager.StartSave();
            Debug.Log("저장되었습니다.");
        });
    }
}
