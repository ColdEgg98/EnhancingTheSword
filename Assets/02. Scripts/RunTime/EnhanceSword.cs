using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnhanceSword : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button enhanceButton;    // 강화 버튼

    private int currentWeaponIndex;

    private void Awake()
    {
        currentWeaponIndex = GameManager.Instance.selectWeaponIndex.Value;
    }

    void Start()
    {
        // 버튼 이벤트 설정
        if (enhanceButton == null) enhanceButton = GetComponentInChildren<Button>();
        enhanceButton.onClick.AddListener(RunEnhancing);

        // 무기 정보 구독
        GameManager.Instance.selectWeaponIndex
            .Subscribe(index => currentWeaponIndex = index)
            .AddTo(this);
    }

    private void RunEnhancing()
    {
        Weapon currentWeapon = GameManager.Instance.currentWeapon.Value;

        // 레벨 상한 체크
        if (currentWeapon.index >= 20)
        {
            Debug.Log("최대 레벨 도달");
            return;
        }

        // 재화 및 요구 아이템 체크
        if (GameManager.Instance.gold.Value < currentWeapon.enhancingPrice)
        {
            Debug.Log("골드 부족");
            return;
        }

        // 재화 소모
        GameManager.Instance.gold.Value -= currentWeapon.enhancingPrice;

        // 강화 시도
        bool result = CheckSuccess(currentWeapon.probability);

        // 강화 결과 처리
        if (result)
            EnhancingSuccessed(currentWeapon);
        else
            EnhancingFailed(currentWeapon);

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

        // 3. 무기 표시 변경
        GameManager.Instance.currentWeapon.Value = newWeapon;

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

        // 2. 무기 표시 꺼짐
        GameManager.Instance.currentWeapon.Value = null;
    }
}