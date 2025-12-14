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
                    targetImage.gameObject.SetActive(false);
                    weaponNameText.gameObject.SetActive(false);
                    InfoText.gameObject.SetActive(false);
                    probabilityText.gameObject.SetActive(false);
                }
                else
                {
                    targetImage.gameObject.SetActive(true);
                    weaponNameText.gameObject.SetActive(true);
                    InfoText.gameObject.SetActive(true);
                    probabilityText.gameObject .SetActive(true);

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
                if (GameManager.Instance.selectWeaponIndex.Value == -1)
                {
                    GameManager.Instance.currentWeapon.Value = null;
                    return;
                }

                GameManager.Instance.currentWeapon.Value = GameManager.Instance.currentData.myWeapons[index];
            })
            .AddTo(this);
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
        if (weapon == null)
        {
            // 파괴됨 알림창
            return;
        }

        probabilityText.text = $"강화 확률 : <color=#FF0000>{weapon.probability}%</color>";

        string tempFormat = $"무기 강화 금액\n\t{ToWonFormat(weapon.enhancingPrice)}\n" +
                            $"필요 아이템\n\t{ListToString(weapon.needItems)}\n" +
                            $"무기 판매 가격\n\t{ToWonFormat(weapon.price)}";
        InfoText.text = tempFormat;
    }

    private string ToWonFormat(long gold)
    {
        if (gold == 0) return "0원";

        long jo = gold / 1000000000000;
        gold %= 1000000000000;
        long eok = gold / 100000000;
        gold %= 100000000;
        long man = gold / 10000;
        gold %= 10000;

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        if (jo > 0)
            sb.Append($"<color=#FFD700>{jo}조</color> ");
        if (eok > 0)
            sb.Append($"<color=#FFD700>{eok}억</color> ");
        if (man > 0)
            sb.Append($"<color=#FFD700>{man}만</color> ");
        if (gold > 0)
            sb.Append($"<color=#FFD700>{gold}</color>");

        sb.Append("원");

        return sb.ToString();
    }

    private string ListToString(List<int> list)
    {
        string str;
        return str = (list == null || list.Count == 0) ? "-" : string.Join(",", list);
    }
}
