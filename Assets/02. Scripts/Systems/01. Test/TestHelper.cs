using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TestHelper : MonoBehaviour
{
    [Header("테스터 버튼")]
    [SerializeField] private GameObject TesterUI;
    [SerializeField] private GameObject Panel;
    private Button btn;

    [Header("버튼 타겟 오브젝트")]
    [SerializeField] private GameObject ButtonsTarget;

    [Header("하위 UI")]
    [SerializeField] private List<GameObject> gameObjects;
    [SerializeField] private List<Button> activeSetter;

    private void Awake()
    {
        #if UNITY_EDITOR
        gameObject.SetActive(true);
        TestAwake();
        #endif
    }

    private void TestAwake()
    {
        btn = GetComponentInChildren<Button>();
        btn.onClick.AddListener(CallTesterUI);

        activeSetter = ButtonsTarget.GetComponentsInChildren<Button>().ToList();
        // 버튼들에 각각의 오브젝트들을 제어하는 함수 부여
        SetGameObjectsActive();
    }

    private void CallTesterUI()
    {
        TesterUI.SetActive(!TesterUI.activeSelf);
        Panel.SetActive(TesterUI.activeSelf);
    }

    private void SetGameObjectsActive()
    {
        Debug.Log("SetGameObjectsActive 실행");
        int index = 0;
        foreach(Button b in activeSetter)
        {
            int currentIndex = index;
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(() => {
                if (gameObjects[currentIndex] != null)
                {
                    gameObjects[currentIndex].SetActive(true);
                    Debug.Log($"현재 index에 저장된 오브젝트 : {currentIndex}, {gameObjects[currentIndex].name}");
                }
                else
                    Debug.LogWarning($"{gameObjects[currentIndex].name}이 비어있음.");
                });
            index++;
        }
    }

/// <summary>각각 X버튼 유니티 이벤트에다 할당. 따로 부여할 수도있지만 귀찮음</summary>
    public void XButton()
    {
        TesterUI.SetActive(false);
        Panel.SetActive(false);
        foreach(GameObject g in gameObjects)
        {
            g.SetActive(false);
        }
    }
}
