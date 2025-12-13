using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnhanceSword : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image targetImage;       // 무기 이미지
    [SerializeField] private TextMeshProUGUI weaponNameText; // 무기 이름
    [SerializeField] private Button enhanceButton;    // 강화 버튼

    private ReactiveProperty<Weapon> selectedWeapon = new ReactiveProperty<Weapon>();

    private int currentWeaponIndex;

    private void Awake()
    {
        currentWeaponIndex = GameManager.Instance.selectWeaponIndex.Value;
    }

    void Start()
    {
        // 1. 버튼 리스너
        if (enhanceButton == null) enhanceButton = GetComponentInChildren<Button>();
        enhanceButton.onClick.AddListener(RunEnhancing);

        // 2. GameManager의 인덱스 변경 감지 -> selectedWeapon 갱신
        GameManager.Instance.selectWeaponIndex
            .Subscribe(index =>
            {
                currentWeaponIndex = index;
                UpdateSelectedWeaponFromData(index);
            })
            .AddTo(this);

        // 3. selectedWeapon 변경 감지 -> 실제 UI 갱신
        selectedWeapon
            .Subscribe(weapon =>
            {
                if (weapon == null)
                {
                    targetImage.gameObject.SetActive(false);
                    weaponNameText.gameObject.SetActive(false);
                }
                else
                {
                    targetImage.gameObject.SetActive(true);
                    weaponNameText.gameObject.SetActive(true);

                    weaponNameText.text = weapon.name;
                    LoadSprite(weapon.addressID);
                }
            })
            .AddTo(this);
    }

    // GameManager 데이터로부터 selectedWeapon 값을 가져오는 헬퍼 함수
    private void UpdateSelectedWeaponFromData(int index)
    {
        var myWeapons = GameManager.Instance.currentData.myWeapons;
        if (myWeapons != null && index >= 0 && index < myWeapons.Count)
        {
            selectedWeapon.Value = myWeapons[index];
        }
        else
        {
            selectedWeapon.Value = null;
        }
    }

    public async void LoadSprite(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        // 로딩 전 깜빡임 방지나 기본값 처리가 필요하다면 여기서 처리

        try
        {
            AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(id);
            Sprite sprite = await handle.Task;

            // 비동기 로드 완료 후, 현재 보여줘야 할 무기가 맞는지 확인 (선택이 그새 바뀌었을 수도 있음)
            if (this != null && selectedWeapon.Value != null && selectedWeapon.Value.addressID == id)
            {
                targetImage.sprite = sprite;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Sprite Load Failed: {e.Message}");
        }
    }

    private void RunEnhancing()
    {
        // Value가 null이면 로직 실행 불가
        if (selectedWeapon.Value == null) return;

        Weapon currentWeapon = selectedWeapon.Value;

        // 1. 만렙 체크
        if (currentWeapon.index >= 20)
        {
            Debug.Log("최대 레벨 도달");
            return;
        }

        // 2. 재화 체크
        if (GameManager.Instance.gold.Value < currentWeapon.enhancingPrice)
        {
            Debug.Log("골드 부족");
            return;
        }

        // 재화 소모
        GameManager.Instance.gold.Value -= currentWeapon.enhancingPrice;

        // 3. 강화 시도
        bool result = CheckSuccess(currentWeapon.probability);

        if (result)
        {
            EnhancingSuccessed(currentWeapon);
        }
        else
        {
            EnhancingFailed(currentWeapon);
        }

        // 저장
        GameManager.Instance.saveDataManager.StartSave();
    }

    private bool CheckSuccess(float p)
    {
        return Random.value * 100 <= p;
    }

    private void EnhancingSuccessed(Weapon currentWeapon)
    {
        // 1. 다음 단계 무기 데이터 가져오기
        int nextIndex = currentWeapon.index + 1;
        if (!GameManager.Instance.allOfWeaponDictionary.ContainsKey(nextIndex)) return;

        Weapon newWeapon = GameManager.Instance.allOfWeaponDictionary[nextIndex];

        // 2. 실제 데이터(GameManager) 갱신
        GameManager.Instance.currentData.myWeapons[currentWeaponIndex] = newWeapon;

        // 3. ReactiveProperty 갱신 -> 구독해둔 UI가 자동으로 바뀜!
        selectedWeapon.Value = newWeapon;

        Debug.Log($"강화 성공: {newWeapon.name}");
    }

    private void EnhancingFailed(Weapon currentWeapon)
    {
        Debug.Log($"{currentWeapon.name} 파괴됨.");

        // 1. 실제 데이터(GameManager)에서 삭제
        var myWeapons = GameManager.Instance.currentData.myWeapons;

        // 리스트에서 제거 (인덱스 밀림 주의)
        if (currentWeaponIndex < myWeapons.Count)
        {
            myWeapons.RemoveAt(currentWeaponIndex);
        }

        // 2. ReactiveProperty 갱신 -> 구독해둔 UI가 자동으로 꺼짐!
        selectedWeapon.Value = null;
    }
}