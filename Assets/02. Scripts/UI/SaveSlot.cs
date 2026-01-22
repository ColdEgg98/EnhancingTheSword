using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI[] previewText;
    [SerializeField] private TextMeshProUGUI[] timeText;

    [Header("Buttons")]
    [SerializeField] private Button[] slotButtons;

    private WrapperForPreviewData wrapper;
    private string dataFormat;

    private void Awake()
    {
        dataFormat = "보유 금액\n{0}\n업적 클리어\n{1}개";
    }

    void Start()
    {
        // 씬 로드 다음에 UIManager를 Init
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.buildIndex == 1)
            {
                GameManager.Instance.uiManager.Init();
                GameManager.Instance.achievementManager.Init();
            }
        };

        SetSlotButtons();
        SetPreviewText();
    }

    /// <summary>슬롯버튼에 SelectSaveSlot를 연결합니다.</summary>
    private void SetSlotButtons()
    {
        slotButtons = GetComponentsInChildren<Button>();
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int index = i;
            slotButtons[index].onClick.AddListener(() => SelectSaveSlot(index));
        }
        Debug.Log("SetSlotButtons is Run");
    }

    /// <summary>슬롯이 클릭됐을 떄 GameManager와 데이터를 주고받습니다.</summary>
    private void SelectSaveSlot(int num)
    {
        // User DataN 파일에서 데이터 로드해서 currentData에 넣어줘야함
        // 로드하는 코드를 SaveDataManager에 작성하고 호출
        GameManager.Instance.saveDataManager.StartLoad(num);

        // isUsed가 false면 true로 바꾸고 첫 처리
        // class 기본값 때문에 빈 세이브면 isUsed가 false임
        if (!GameManager.Instance.currentData.previewData.isUsed)
        {
            GameManager.Instance.currentData.previewData.isUsed = true;
            // 기본금 + 기본 무기 지급
            GameManager.Instance.gold.Value = 5000000;
            GameManager.Instance.currentData.myWeapons = new();
            Weapon woodSword = GameManager.Instance.allOfWeaponDictionary[1];
            GameManager.Instance.currentData.myWeapons.Add(woodSword); // 목검
            GameManager.Instance.saveDataManager.StartSave(num);

            // 게임 실행 업적
            _ = GameManager.Instance.achievementManager.GameStartAchieved();
        }

        // 화면 바뀔 때 무기 표시
        if (GameManager.Instance.currentData.myWeapons.Count > 0)
            GameManager.Instance.currentWeapon.Value = GameManager.Instance.currentData.myWeapons[0];
        else if (GameManager.Instance.currentData.myWeapons.Count == 0)
            GameManager.Instance.selectWeaponIndex.Value = -1;

        SceneManager.LoadScene(1);
    }

    /// <summary>UI에 표시되는 데이터를 변경합니다</summary>
    private void SetPreviewText()
    {
        wrapper = GameManager.Instance.saveDataManager.wrapperPreviewData;
        int index = 0;
        foreach (PreviewData data in wrapper.slots)
        {
            if (!data.isUsed)
            {
                previewText[index].text = "눌러서\n게임 시작";
                timeText[index].text = "-";
            }
            else
            {
                previewText[index].text = string.Format(dataFormat, data.goldRef, data.achivementCount);
                timeText[index].text = data.time;
            }
            index++;
        }
    }
}
