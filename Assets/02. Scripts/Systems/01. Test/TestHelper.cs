using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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

    [Header("골드 획득 UI")]
    [SerializeField] private TMP_InputField goldAmountInput;

    [Header("무기 획득 UI")]
    [SerializeField] private TMP_InputField weaponIndexInput;

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

        if (goldAmountInput == null)
            Debug.LogWarning("goldInputField가 연결되지 않았습니다.");
        else
            goldAmountInput.onSubmit.AddListener(OnSubmitGoldInput);
        
        if (weaponIndexInput == null)
            Debug.LogWarning("weaponIndexInput가 연결되지 않았습니다.");
        else
            weaponIndexInput.onSubmit.AddListener(OnSubmitWeaponIndexInput);
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

    public void OnSubmitGoldInput(string amount)
    {
        GameManager.Instance.userDataManager.GetGold(amount);
        XButton();
    }

    public void OnSubmitWeaponIndexInput(string amount)
    {
        GameManager.Instance.userDataManager.GetWeapon(amount);
        XButton();
    }

/// <summary>각각 X버튼 유니티 이벤트에다 할당. 따로 부여할 수도있지만 귀찮음</summary>
    public void XButton()
    {
        TesterUI.SetActive(false);
        Panel.SetActive(false);
        foreach(GameObject g in gameObjects)
        {
            if (g == null)
            {
                Debug.LogWarning("리스트 할당이 되지않은 오브젝트가 있습니다.");
            }
            else
                g.SetActive(false);
        }
    }
}
