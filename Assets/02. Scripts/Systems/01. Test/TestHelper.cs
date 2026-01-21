using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

    [Header("InputFields")]
    [SerializeField] private TMP_InputField goldAmountInput;
    [SerializeField] private TMP_InputField weaponIndexInput;
    [SerializeField] private TMP_InputField toastInput;
    [SerializeField] private TMP_InputField NoticeInput;

    [Header("Toggle")]
    [SerializeField] private Toggle colorToggle;
    private Color c;

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

        c = Color.white;

        activeSetter = ButtonsTarget.GetComponentsInChildren<Button>().ToList();
        // 버튼들에 각각의 오브젝트들을 제어하는 함수 부여
        SetGameObjectsActive();

        InputFieldsInit();
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
        foreach (Button b in activeSetter)
        {
            int currentIndex = index;
            Debug.Log($"현재 index에 저장된 오브젝트 : {currentIndex}번 : {gameObjects[currentIndex].name}");
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(() =>
            {
                if (gameObjects[currentIndex] != null)
                {
                    gameObjects[currentIndex].SetActive(true);
                }
                else
                    Debug.LogWarning($"{gameObjects[currentIndex].name}이 비어있음.");
            });
            index++;
        }
    }

    private void InputFieldsInit()
    {
        goldAmountInput.onSubmit.AddListener(OnSubmitGoldInput);
        weaponIndexInput.onSubmit.AddListener(OnSubmitWeaponIndexInput);
        toastInput.onSubmit.AddListener(OnSubmitToastMesage);
        NoticeInput.onSubmit.AddListener(OnSubmitNoticeMesage);

        colorToggle.onValueChanged.AddListener(colorChange);
    }

    private void colorChange(bool arg0)
    {
        c = (arg0) ? Color.white : Color.red;
    }

    public void OnSubmitGoldInput(string amount)
    {
        GameManager.Instance.userDataManager.GetGold(amount);
        goldAmountInput.text = string.Empty;
        XButton();
    }

    public void OnSubmitWeaponIndexInput(string amount)
    {
        GameManager.Instance.userDataManager.GetWeapon(amount);
        weaponIndexInput.text = string.Empty;
        XButton();
    }

    public void OnSubmitToastMesage(string mesage)
    {
        GameManager.Instance.uiManager.UIFactory.ShowToast(mesage);
        toastInput.text = string.Empty;
        XButton();
    }

    public void OnSubmitNoticeMesage(string mesage)
    {
        GameManager.Instance.uiManager.UIFactory.ShowNotice(mesage, c);
        NoticeInput.text = string.Empty;
        XButton();
    }

    /// <summary>각각 X버튼 유니티 이벤트에다 할당. 따로 부여할 수도있지만 귀찮음</summary>
    public void XButton()
    {
        TesterUI.SetActive(false);
        Panel.SetActive(false);
        foreach (GameObject g in gameObjects)
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
