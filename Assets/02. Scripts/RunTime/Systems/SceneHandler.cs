using System.Collections.Generic;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image targetImage; // 무기 이미지
    [SerializeField] private TextMeshProUGUI weaponNameText; // 무기 이름
    [SerializeField] private TextMeshProUGUI probabilityText; // 강화 확률
    [SerializeField] private TextMeshProUGUI InfoText; // 무기 강화 & 판매 정보 표기

    private void Start()
    {
        SetSubscribe();
    }

    private void SetSubscribe()
    {
        GameManager.Instance.currentWeapon
            .Subscribe(weapon =>
            {
                if (weapon == null)
                {
                    GameObjectsSetActive(false);
                }
                else
                {
                    GameObjectsSetActive(true);

                    weaponNameText.text = weapon.name;
                    LoadSprite(weapon.addressID);
                }

                // 무기 정보(강화 비용 등) 갱신
                UpdateWeaponInfo(weapon);
            })
            .AddTo(this);

        // 인벤토리 등 무기 변경시 스프라이트 변경
        GameManager.Instance.selectWeaponIndex
            .Subscribe(index =>
            {
                if (GameManager.Instance.selectWeaponIndex.Value <= -1)
                {
                    GameManager.Instance.currentWeapon.Value = null;
                    return;
                }

                GameManager.Instance.currentWeapon.Value = GameManager.Instance.currentData.myWeapons[index];
            })
            .AddTo(this);
    }

    private void GameObjectsSetActive(bool v)
    {
        targetImage.gameObject.SetActive(v);
        weaponNameText.gameObject.SetActive(v);
        InfoText.gameObject.SetActive(v);
        probabilityText.gameObject.SetActive(v);
    }

    public async void LoadSprite(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        try
        {
            AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(id);
            Sprite sprite = await handle.Task;

            targetImage.sprite = sprite;
        }
        catch (Exception e)
        {
            Debug.LogError($"Sprite Load Failed: {e.Message}");
        }
    }

    public void UpdateWeaponInfo(Weapon weapon)
    {
        // 파괴
        if (GameManager.Instance.selectWeaponIndex.Value == -1)
        {
            GameManager.Instance.uiManager.UIFactory.ShowNotice("무기가 파괴되었습니다", Color.red);
            return;
        }

        // 판매
        if (GameManager.Instance.selectWeaponIndex.Value == -2)
            return;

        if (weapon.index == 20)
        {
            probabilityText.text = "마지막 단계에 도달했습니다.";
            InfoText.text = $"무기 강화 금액\n\t-\n" +
                            $"필요 아이템\n\t-\n" +
                            $"무기 판매 가격\n\t{StrUtiity.ToWonFormat(weapon.price)}";
            return;
        }

        float Bonus = GameManager.Instance.currentData.chanceBonus;

        probabilityText.text = $"강화 확률 : <color=#FF0000>{weapon.probability}%</color>";
        if (Bonus != 0f)
        probabilityText.text += $" + <color=#FFD700>({Bonus}%)</color>";

        string tempFormat = $"무기 강화 금액\n\t{StrUtiity.ToWonFormat(weapon.enhancingPrice)}\n" +
                            $"필요 아이템\n\t{ListToString(weapon.needItems)}\n" +
                            $"무기 판매 가격\n\t{StrUtiity.ToWonFormat(weapon.price)}";
        InfoText.text = tempFormat;
    }

    private string ListToString(List<int> list)
    {
        string str = (list == null || list.Count == 0) ? "-" : string.Join(",", list);
        return str;
    }
}
