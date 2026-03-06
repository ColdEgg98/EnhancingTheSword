using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryButtonBehavior : MonoBehaviour
{
    // 무기 컨텐츠 그룹 하나 잡기
    // myWeapon.count만큼 인벤토리 보이게 투명도 255 + 무기 사진 로드
    // 그리고 인벤토리 UI 활성화

    [Header("UIElements")]
    public GameObject InventoryPanel;
    [SerializeField] private Image[] Contents;
    [SerializeField] private TextMeshProUGUI SellText;

    [Header("Buttons")]
    [SerializeField] private Button XButton;
    public Button multiSellButton;

    [Header("Switch")]
    [SerializeField] private Button SwitchCategoryButton;
    [SerializeField] private TextMeshProUGUI SwitchText;
    [SerializeField] private Image SwitchImage;

    public List<IViewable> myIViewables;
    public bool isWeaponCategory = true;
    public ReactiveDictionary<int, IViewable> IViewableForSell;

    [Header("Drag And Drop")]
    [SerializeField] private Image ghostSprite;
    private int onClickItemIndex;

    private void Awake()
    {
        XButton.onClick.AddListener(() => OnClickXButton(false));
        myIViewables = GameManager.Instance.currentData.myWeapons.OfType<IViewable>().ToList();
        IViewableForSell = new();
        onClickItemIndex = -1;
        ghostSprite.color = new Color(1, 1, 1, 0.75f);
        ghostSprite.raycastTarget = false;
        ghostSprite.enabled = false;
    }

    private void Start()
    {
        // 다중 판매 팁 텍스트
        IViewableForSell
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

        SwitchCategoryButton.onClick.AddListener(() => SwitchCategory().Forget());

        // 우클릭 이벤트는 유니티 기본 버튼에서 지원안하므로 UniRx 사용
        RightDragAndDropSub();
    }

    private void RightDragAndDropSub()
    {
        foreach (var image in Contents)
        {
            image.OnBeginDragAsObservable()
                .Where(pointEvent => pointEvent.button == PointerEventData.InputButton.Right)
                .Subscribe(_ =>
                {
                    // 선택한 이미지 UI에 따라 switchItemValue를 설정
                    RightClickPress();
                })
                .AddTo(this);

            image.OnDragAsObservable()
                .Where(pointEvent => pointEvent.button == PointerEventData.InputButton.Right)
                .Subscribe(_ =>
                {
                    GhostChaseMouse();
                })
                .AddTo(this);

            image.OnEndDragAsObservable()
                .Where(pointEvent => pointEvent.button == PointerEventData.InputButton.Right)
                .Subscribe(_ =>
                {
                    RightClickRelease();
                })
                .AddTo(this);
        }
    }

    // 카테고리 전환 + UI 변경
    public async UniTask SwitchCategory()
    {
        Debug.Log("✅ SwitchCategory is Run");
        isWeaponCategory = !isWeaponCategory;
        string AAstring = isWeaponCategory ? "SwitchMaterial" : "SwitchWeapon";
        SwitchText.text = isWeaponCategory ? "재료 보기" : "무기 보기";
        await GameManager.Instance.aAResourceManager.SetSpriteAsync(AAstring, SwitchImage);
        OnClickXButton(true);
        await OnClickButton();
    }

    // 인스펙터 할당용. OnClickButton이 UniTask이기 때문에 유니티 이벤트 반환 타입과 안 맞음
    public void OnClickButton_Inspector()
    {
        OnClickButton().Forget();
    }

    public async UniTask OnClickButton()
    {
        if (!InventoryPanel.activeSelf)
        {
            if (isWeaponCategory)
                myIViewables = GameManager.Instance.currentData.myWeapons.OfType<IViewable>().ToList();
            else
                myIViewables = GameManager.Instance.currentData.materials.OfType<IViewable>().ToList();

            await ButtonBehavior(myIViewables);
        }
        else
            OnClickXButton();
    }

    public async UniTask ButtonBehavior(List<IViewable> viewAbles)
    {
        // 4. 효과음 재생
        GameManager.Instance.soundManager.PlaySFX("OpenBag");

        int count = viewAbles.Count;

        // 1. 모든 작업을 리스트에 담음
        List<UniTask> loadingTasks = new List<UniTask>();

        for (int i = 0; i < count; i++)
        {
            int index = i;
            // 모든 로딩을 동시에 시작시키고, 그 '작업(Task)' 자체를 리스트에 저장
            loadingTasks.Add(ImageChange(index, viewAbles));
        }

        // 2. 모든 작업이 끝날 때까지 여기서 대기
        await UniTask.WhenAll(loadingTasks);

        // 3. 여기까지 오면 모든 이미지가 100% 로딩 완료된 상태임
        Debug.Log("모든 무기 이미지 로딩 완료");

        InventoryPanel.SetActive(true);

        // 팁 변경
        GameManager.Instance.uiManager.TipTextAppend("가방 닫기 (E)");
        GameManager.Instance.uiManager.TipTextAppend("현재 무기 변경 (무기 이미지 클릭)");
        GameManager.Instance.uiManager.TipTextAppend("다중 선택 (Shift + 클릭)");
        GameManager.Instance.uiManager.TipTextSub("가방 열기 (E)");
    }

    public async UniTask ImageChange(int index, List<IViewable> IViewables)
    {
        if (index >= 20)
            return;

        await GameManager.Instance.aAResourceManager.SetSpriteAsync(IViewables[index], Contents[index]);
        Contents[index].color = Color.white;
        Contents[index].raycastTarget = true;
        Contents[index].preserveAspect = !isWeaponCategory;

        // Button 연결
        Button tempBtn;
        tempBtn = Contents[index].GetComponent<Button>();
        tempBtn.onClick.RemoveAllListeners();
        if (isWeaponCategory)
            tempBtn.onClick.AddListener(() => ContentButtonBehavior(index));
        else
            tempBtn.onClick.AddListener(() => ItemBehavior(index));
    }

    public void ContentButtonBehavior(int index)
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
        RectTransform rect = Contents[index].gameObject.GetComponent<RectTransform>();
        if (!IViewableForSell.ContainsKey(index))
        {
            IViewableForSell.Add(index, myIViewables[index]);
            GameManager.Instance.redSquareManager.RedSquareGenerater(rect);
        }
        else
        {
            IViewableForSell.Remove(index);
            GameManager.Instance.redSquareManager.RedSquareRemover(rect);
        }
        Debug.Log($"acitveRedSquare Count : {IViewableForSell.Count}");
    }

    // Drag And Drop
    private void RightClickPress()
    {
        // 고스트 이미지 띄우기
        // 클릭된 오브젝트의 스프라이트 받기
        GameObject AAsprite = GetMouseRayObject();
        ghostSprite.sprite = AAsprite.GetComponent<Image>().sprite;
        if (ghostSprite.sprite == null) Debug.LogWarning("ghostSprite.sprite is null");
        Vector2 mousePos = Mouse.current.position.ReadValue();

        onClickItemIndex = int.Parse(AAsprite.gameObject.name);
        ghostSprite.enabled = true;
    }

    private void GhostChaseMouse()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        ghostSprite.transform.position = mousePos;
    }

    private void RightClickRelease()
    {
        // 위치와 인덱스 변경
        // ❗ 현재 구조는 이름을 인덱스로 쓰고있기 때문에, 다른 UI오브젝트 이름이 그냥 숫자면 위험
        GameObject mouseObject = GetMouseRayObject();
        if (int.TryParse(mouseObject.name, out int targetIndex))
        {
            List<Weapon> myWeapons = GameManager.Instance.GetMyWeapons();

            // 만약 빈칸에서 Release 됐을 때 처리
            if (targetIndex > myWeapons.Count)
            {
                onClickItemIndex = -1;
                ghostSprite.enabled = false;
                return;
            }

            Weapon tempWeapon = myWeapons[targetIndex];
            myWeapons[targetIndex] = myWeapons[onClickItemIndex];
            myWeapons[onClickItemIndex] = tempWeapon;

            // 복사 방지
            if (onClickItemIndex == GameManager.Instance.selectWeaponIndex.Value)
            {
                GameManager.Instance.selectWeaponIndex.Value = targetIndex;
            }
            else if (targetIndex == GameManager.Instance.selectWeaponIndex.Value)
            {
                GameManager.Instance.selectWeaponIndex.Value = onClickItemIndex;
            }

            ButtonBehavior(GameManager.Instance.currentData.myWeapons.OfType<IViewable>().ToList()).Forget();
        }
        else Debug.LogWarning($"index change failed : {mouseObject.name}");

        onClickItemIndex = -1;
        ghostSprite.enabled = false;
    }

    // 마우스 포지션과 겹친 첫번째(제일 위에 표시되는) 오브젝트 반환
    private GameObject GetMouseRayObject()
    {
        GameObject temp = null;
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(pointerEventData, results);
        if (results.Count > 0)
        {
            RaycastResult result = results[0];
            temp = result.gameObject;
        }
        return temp;
    }

    private void ItemBehavior(int index)
    {
        if (Keyboard.current.shiftKey.isPressed)
        {
            ShiftClickEvent(index);
            return;
        }

        MaterialItem item = GameManager.Instance.currentData.materials[index];
        if (item.action.IsValid(item))
        {
            item.action.Excute(item);
            GameManager.Instance.currentData.materials.RemoveAt(index);
            OnClickXButton();
        }
    }

    public void OnClickXButton(bool isSwitching = false)
    {
        // 인벤 내부 정보 리셋
        for (int i = 0; i < Contents.Length; i++)
        {
            int index = i;
            if (Contents == null)
                continue;
            Color c = Contents[index].color;
            Contents[index].color = Color.clear;
            Contents[index].raycastTarget = false;
        }
        // RedSquare가 있다면 해제
        RemoveRedSquares();

        IViewableForSell.Clear();
        myIViewables.Clear();
        GameManager.Instance.aAResourceManager.ReleseAllAssets();
        InventoryPanel.SetActive(false);
        if (!isSwitching) GameManager.Instance.soundManager.PlaySFX("OpenBag");

        // 팁 변경
        GameManager.Instance.uiManager.TipTextAppend("가방 열기 (E)");
        GameManager.Instance.uiManager.TipTextSub("현재 무기 변경 (무기 이미지 클릭)");
        GameManager.Instance.uiManager.TipTextSub("다중 선택 (Shift + 클릭)");
        GameManager.Instance.uiManager.TipTextSub("가방 닫기 (E)");
    }

    private void RemoveRedSquares()
    {
        RedSquare[] reds = InventoryPanel.GetComponentsInChildren<RedSquare>();
        if (reds.Length == 0) return;

        foreach (RedSquare red in reds)
        {
            RectTransform rect = red.gameObject.GetComponent<RectTransform>();
            GameManager.Instance.redSquareManager.RedSquareRemover(rect);
        }
    }
}
