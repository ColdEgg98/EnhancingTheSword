using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnhanceSword : MonoBehaviour
{
    // 모루 컴포넌트로 사용
    private Button button;
    private Weapon selectedWeapon;
    List<Weapon> myWeapons;

    void Awake()
    {
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(RunEnhancing);
    }

    private void RunEnhancing()
    {
        // 현재 무기 정보 세팅
        SetWeaponValue();
        if (selectedWeapon.index == 20)
            return;

        // 재화 소모
        Debug.Log($"{GameManager.Instance.currentData.previewData.goldRef}에서 {selectedWeapon.enhancingPrice}만큼 차감됨");
        GameManager.Instance.currentData.previewData.goldRef -= selectedWeapon.enhancingPrice;
        Debug.Log($"남은 골드 : {GameManager.Instance.currentData.previewData.goldRef}");
        // 확률 따라서 통과 시
        bool result = CheckSuccess(selectedWeapon.probability);
        // 파티클 이펙트

        // 무기 인덱스 다음 껄로 변환
        if (result)
            EnhancingSuccessed();
        else
            myWeapons.Remove(selectedWeapon);

        // 저장
        GameManager.Instance.saveDataManager.StartSave();
    }

    private void SetWeaponValue()
    {
        myWeapons = GameManager.Instance.currentData.myWeapons;
        int selectIndex = GameManager.Instance.selectWeaponIndex;

        selectedWeapon = myWeapons[selectIndex];
    }

    private bool CheckSuccess(float p)
    {
        return Random.value * 100 <= p;
    }

    private void EnhancingSuccessed()
    {
        Weapon newWeapon = GameManager.Instance.allOfWeaponDictionary[selectedWeapon.index + 1];
        int listIndex = myWeapons.IndexOf(selectedWeapon);
        myWeapons[listIndex] = newWeapon;
        // 씬의 이미지랑 이름 표기 바꿔줘야함
        Debug.Log($"RunEnhancing : Weapon Enhanced\n{selectedWeapon.name}이 {newWeapon.name}으로 강화됨.");
    }
}
