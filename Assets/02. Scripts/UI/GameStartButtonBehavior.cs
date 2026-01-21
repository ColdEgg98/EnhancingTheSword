using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임 시작 클릭했을 때 세이브 슬롯 UI 띄우는 클래스
/// </summary>
public class GameStartButtonBehavior : MonoBehaviour
{
    [SerializeField] private GameObject saveSlotUI;
    Button startButton;

    private void Awake()
    {
        startButton = GetComponent<Button>();
    }

    void Start()
    {
        startButton.onClick.AddListener(StartButtonBehavior);
    }

    void StartButtonBehavior()
    {
        saveSlotUI.SetActive(true);
    }
}
