using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryButtonBehavior : MonoBehaviour
{
    // 무기 컨텐츠 그룹 하나 잡기
    // myWeapon.count만큼 인벤토리 보이게 투명도 255 + 무기 사진 로드
    // 그리고 인벤토리 UI 활성화

    [SerializeField]
    private Image[] weaponContents;
    public GameObject inventory;
    [SerializeField] private Button XButton;
    [SerializeField] private TextMeshProUGUI SellText;
    public Button multiSellButton;
    private List<Weapon> myWeapons;
    public ReactiveDictionary<int, Weapon> weaponsForSell;

    private void Awake()
    {
        XButton.onClick.AddListener(OnClickXButton);
        myWeapons = GameManager.Instance.currentData.myWeapons;
        weaponsForSell = new();

        weaponsForSell
            .ObserveCountChanged()
            .Subscribe(count =>
            {
                if (count != 0)
                {
                    multiSellButton.interactable = true;
                    Color c = SellText.color;
                    c.a = 1f;
                    SellText.color = c;
                    GameManager.Instance.uiManager.TipTextAppend("선택 목록 판매 (S)");
                    GameManager.Instance.uiManager.TipTextSub("판매 (S)");
                }
                else
                {
                    multiSellButton.interactable = false;
                    Color c = SellText.color;
                    c.a = 0.35f;
                    SellText.color = c;
                    GameManager.Instance.uiManager.TipTextSub("선택 목록 판매 (S)");
                    GameManager.Instance.uiManager.TipTextAppend("판매 (S)");
                }
                    
            })
            .AddTo(this);
    }

    public async void OnClickButton()
    {
        if (!inventory.activeSelf)
            await ButtonBehavior();
        else
            OnClickXButton();
    }

    public async Task ButtonBehavior()
    {
        int count = myWeapons.Count;

        // 1. 모든 작업을 리스트에 담음
        List<Task> loadingTasks = new List<Task>();

        for (int i = 0; i < count; i++)
        {
            int index = i;
            // 모든 로딩을 동시에 시작시키고, 그 '작업(Task)' 자체를 리스트에 저장
            loadingTasks.Add(ImageChange(index));
        }

        // 2. 모든 작업이 끝날 때까지 여기서 대기
        await Task.WhenAll(loadingTasks);

        // 3. 여기까지 오면 모든 이미지가 100% 로딩 완료된 상태임
        Debug.Log("모든 무기 이미지 로딩 완료");

        inventory.SetActive(true);

        // 팁 변경
        GameManager.Instance.uiManager.TipTextAppend("가방 닫기 (E)");
        GameManager.Instance.uiManager.TipTextAppend("현재 무기 변경 (무기 이미지 클릭)");
        GameManager.Instance.uiManager.TipTextAppend("다중 선택 (Shift + 클릭)");
        GameManager.Instance.uiManager.TipTextSub("가방 열기 (E)");
    }

    public async Task ImageChange(int index)
    {
        await myWeapons[index].ApplySpriteToImage(weaponContents[index]);
        weaponContents[index].color = Color.white;
        weaponContents[index].raycastTarget = true;

        // Button 연결
        Button tempBtn;
        tempBtn = weaponContents[index].GetComponent<Button>();
        tempBtn.onClick.RemoveAllListeners();
        tempBtn.onClick.AddListener(() => WeaponContentButtonBehavior(index));
    }

    public void WeaponContentButtonBehavior(int index)
    {
        if (Keyboard.current.shiftKey.isPressed)
        {
            ShiftClickEvent(index);
            return;
        }

        // 클릭하면 인벤 닫히면서 메인 화면 무기 바꾸기
        Debug.Log($"인덱스 변경 {GameManager.Instance.selectWeaponIndex.Value} -> {index}");
        GameManager.Instance.selectWeaponIndex.Value = index;
        OnClickXButton();
    }

    private void ShiftClickEvent(int index)
    {
        RectTransform rect = weaponContents[index].gameObject.GetComponent<RectTransform>();
        if (!weaponsForSell.ContainsKey(index))
        {
            weaponsForSell.Add(index, myWeapons[index]);
            GameManager.Instance.redSquareManager.RedSquareGenerater(rect);
        }
        else
        {
            weaponsForSell.Remove(index);
            GameManager.Instance.redSquareManager.RedSquareRemover(rect);
        }
        Debug.Log($"acitveRedSquare Count : {weaponsForSell.Count}");
    }

    public void OnClickXButton()
    {
        int index;
        // 인벤 내부 정보 리셋
        for (int i = 0; i < weaponContents.Length; i++)
        {
            index = i;
            if (weaponContents == null)
                continue;
            Color c = weaponContents[index].color;
            weaponContents[index].color = Color.clear;
            weaponContents[index].raycastTarget = false;
        }
        GameManager.Instance.redSquareManager.RedSquareAllRemover();
        weaponsForSell.Clear();
        inventory.SetActive(false);
        
        // 팁 변경
        GameManager.Instance.uiManager.TipTextAppend("가방 열기 (E)");
        GameManager.Instance.uiManager.TipTextSub("현재 무기 변경 (무기 이미지 클릭)");
        GameManager.Instance.uiManager.TipTextSub("다중 선택 (Shift + 클릭)");
        GameManager.Instance.uiManager.TipTextSub("가방 닫기 (E)");
    }
}
