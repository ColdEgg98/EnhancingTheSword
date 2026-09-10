using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{
    [Header("UIElements")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Image[] contents;

    [Header("Ghost (드래그 미리보기)")]
    [SerializeField] private Image ghostSprite; // 🔧 인스펙터 연결 필수 (원본은 null이라 Awake에서 NRE)

    [Header("Buttons")]
    [SerializeField] private Button xButton;
    [SerializeField] private Button panel;
    [SerializeField] private Button switchCategoryButton;

    private int onClickItemIndex = -1;

    public event Action OnCloseClicked;
    public event Action OnSwitchCategoryClicked;
    public event Action<int> OnSlotClicked;
    public event Action<int, int> OnItemSwapped; // originIndex, targetIndex

    public bool IsPanelActive => inventoryPanel.activeSelf;
    public Image GetSlotImage(int index) => contents[index];

    private void Awake()
    {
        xButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
        panel.onClick.AddListener(() => OnCloseClicked?.Invoke());
        switchCategoryButton.onClick.AddListener(() => OnSwitchCategoryClicked?.Invoke());

        if (ghostSprite != null)
        {
            ghostSprite.color = new Color(1, 1, 1, 0.75f);
            ghostSprite.raycastTarget = false;
            ghostSprite.enabled = false;
        }
    }

    private void Start()
    {
        RightDragAndDropSub();
    }

    // ── 활성/비활성 ───────────────────────────────
    // 팁 텍스트까지 세팅하는 "정식 열기" — 인벤토리를 처음 열 때만 호출
    public void ShowInventory()
    {
        inventoryPanel.SetActive(true);

        GameManager.Instance.uiManager.TipTextAppend("가방 닫기 (E)");
        GameManager.Instance.uiManager.TipTextAppend("현재 무기 변경 (무기 이미지 클릭)");
        GameManager.Instance.uiManager.TipTextSub("가방 열기 (E)");
    }

    // 팁은 건드리지 않고 패널만 토글 — 카테고리 전환용
    public void SetPanelActive(bool value) => inventoryPanel.SetActive(value);

    // 슬롯 시각 초기화만 (전환/재로딩 직전 재사용)
    public void ClearSlots()
    {
        foreach (var content in contents)
        {
            if (content == null) continue;
            content.color = Color.clear;
            content.raycastTarget = false;
        }
    }

    // 팁 정리 + 슬롯 정리 + 패널 끄기 — 인벤토리를 완전히 닫을 때만 호출
    public void HideInventory()
    {
        ClearSlots();
        inventoryPanel.SetActive(false);

        GameManager.Instance.uiManager.TipTextAppend("가방 열기 (E)");
        GameManager.Instance.uiManager.TipTextSub("현재 무기 변경 (무기 이미지 클릭)");
        GameManager.Instance.uiManager.TipTextSub("가방 닫기 (E)");
    }

    // ── 슬롯 세팅 ───────────────────────────────
    public void SetupSlot(int index, bool preserveAspect)
    {
        contents[index].color = Color.white;
        contents[index].raycastTarget = true;
        contents[index].preserveAspect = preserveAspect;

        Button btn = contents[index].GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => OnSlotClicked?.Invoke(index));
    }

    // AA 재로딩 없이 스프라이트만 즉시 교환 (순서 바꾸기용)
    public void SwapSlotSprites(int a, int b)
    {
        (contents[a].sprite, contents[b].sprite) = (contents[b].sprite, contents[a].sprite);
    }

    #region Drag And Drop (UI 순수 시각 처리)
    private void RightDragAndDropSub()
    {
        foreach (var image in contents)
        {
            image.OnBeginDragAsObservable()
                .Where(pointEvent => pointEvent.button == PointerEventData.InputButton.Right)
                .Subscribe(_ => RightClickPress())
                .AddTo(this);

            image.OnDragAsObservable()
                .Where(pointEvent => pointEvent.button == PointerEventData.InputButton.Right)
                .Subscribe(_ => GhostChaseMouse())
                .AddTo(this);

            image.OnEndDragAsObservable()
                .Where(pointEvent => pointEvent.button == PointerEventData.InputButton.Right)
                .Subscribe(_ => RightClickRelease())
                .AddTo(this);
        }
    }

    private void RightClickPress()
    {
        GameObject mouseObj = GetMouseRayObject();
        if (mouseObj == null) return;

        ghostSprite.sprite = mouseObj.GetComponent<Image>().sprite;
        onClickItemIndex = int.Parse(mouseObj.name);
        ghostSprite.enabled = true;
    }

    private void GhostChaseMouse()
    {
        ghostSprite.transform.position = Mouse.current.position.ReadValue();
    }

    private void RightClickRelease()
    {
        GameObject mouseObject = GetMouseRayObject();
        if (mouseObject != null && int.TryParse(mouseObject.name, out int targetIndex))
        {
            // 교환 로직은 Presenter로 위임
            OnItemSwapped?.Invoke(onClickItemIndex, targetIndex);
        }

        onClickItemIndex = -1;
        ghostSprite.enabled = false;
    }

    private GameObject GetMouseRayObject()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        { position = Mouse.current.position.ReadValue() };
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(pointerData, results);
        return results.Count > 0 ? results[0].gameObject : null;
    }
    #endregion
}