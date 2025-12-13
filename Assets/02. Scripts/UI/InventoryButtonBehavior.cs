using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButtonBehavior : MonoBehaviour
{
    // 무기 컨텐츠 그룹 하나 잡기
    // myWeapon.count만큼 인벤토리 보이게 투명도 255 + 무기 사진 로드
    // 그리고 인벤토리 UI 활성화

    [SerializeField]
    private Image[] weaponContents;
    [SerializeField]
    private GameObject inventory;
    [SerializeField]
    private Button XButton;
    private bool isWeaponSold;
    private List<Weapon> myWeapons;

    private void Awake()
    {
        XButton.onClick.AddListener(OnClickXButton);
        myWeapons = GameManager.Instance.currentData.myWeapons;
    }

    public async void OnClickButton()
    {
        await ButtonBehavior();
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
        Debug.Log("모든 무기 이미지 로딩 완료!");

        inventory.SetActive(true);
    }

    public async Task ImageChange(int index)
    {
        await myWeapons[index].ApplySpriteToImage(weaponContents[index]);
        weaponContents[index].color = Color.white;
        weaponContents[index].raycastTarget = true;

        // 커서 올리면 무기 이름 뜨기

        // Button 연결
        Button tempBtn;
        tempBtn = weaponContents[index].GetComponent<Button>();
        tempBtn.onClick.AddListener(() => WeaponContentButtonBehavior(index));
    }

    public void OnClickXButton()
    {
        // 인벤 내부 정보 리셋
        for (int i = 0; i < myWeapons.Count; i++)
        {
            int index = i;
            Color c = weaponContents[index].color;
            weaponContents[index].color = Color.clear;
            weaponContents[index].raycastTarget = false;
        }
        inventory.SetActive(false);
    }

    public void WeaponContentButtonBehavior(int index)
    {
        // 클릭하면 인벤 닫히면서 메인 화면 무기 바꾸기
        GameManager.Instance.selectWeaponIndex.Value = index;
        OnClickXButton();
    }
}
