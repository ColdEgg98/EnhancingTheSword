using System.Collections.Generic;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using DG.Tweening;

public class SceneHandler : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image targetImage; // 무기 이미지
    private Material _material;
    private int _flashID;
    [SerializeField] private TextMeshProUGUI weaponNameText; // 무기 이름
    [SerializeField] private TextMeshProUGUI probabilityText; // 강화 확률
    [SerializeField] private TextMeshProUGUI InfoText; // 무기 강화 & 판매 정보 표기
    [SerializeField] private TextMeshProUGUI TipText; // 좌상단에 튜토리얼
    [SerializeField] private GameObject TesterButton; // 테스트 헬퍼

    private void Awake()
    {
        _flashID = Shader.PropertyToID("_FlashAmount");
        _material = targetImage.material;
        SetSubscribe();
    }

    private void Start()
    {
        GameManager.Instance.uiManager.TipTextAppend("강화 하기 (Space)");
        GameManager.Instance.uiManager.TipTextAppend("가방 열기 (E)");
        GameManager.Instance.uiManager.TipTextAppend("판매 (S)");

        GameManager.Instance.soundManager.PlayBGM("578910__imania414__retro-80-s");
#if UNITY_EDITOR
        TesterButton.SetActive(true);
#endif
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

                    LevelBGM(weapon.index);

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
                // 강화 중 무기 변경 시 연출 초기화
                _material.DOKill();
                _material.SetFloat(_flashID, 0f);

                GameManager.Instance.currentWeapon.Value = GameManager.Instance.currentData.myWeapons[index];
            })
            .AddTo(this);

        // 좌상단 팁 뜨는거 관리
        GameManager.Instance.uiManager.tipList
            .ObserveCountChanged()
            .Subscribe(_ =>
            {
                string stringFormat = string.Join("\n", GameManager.Instance.uiManager.tipList);
                TipText.text = stringFormat;
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

    private void LevelBGM(int weaponLevel)
    {
        if (weaponLevel < 13)
        {
            if (GameManager.Instance.GetCurrentBGMName() == "715708__prodbyrey__retro-game-music-loop")
                GameManager.Instance.StopBGM();

            string bgmName = string.Empty;
            int num = UnityEngine.Random.Range(0, 3);
            switch (num)
            {
                case 0:
                    bgmName = "578910__imania414__retro-80-s";
                    break;
                case 1:
                    bgmName = "685934__timouse__techno-beat-cargo";
                    break;
                case 2:
                    bgmName = "721948__audiocoffee__creative-background-loop-ver";
                    break;
            }
            GameManager.Instance.PlayBGM(bgmName);
        }
        else if (weaponLevel >= 13)
        {
            if (GameManager.Instance.GetCurrentBGMName() != "715708__prodbyrey__retro-game-music-loop")
                GameManager.Instance.StopBGM();

            GameManager.Instance.StopBGM();
            GameManager.Instance.PlayBGM("715708__prodbyrey__retro-game-music-loop");
        }
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
