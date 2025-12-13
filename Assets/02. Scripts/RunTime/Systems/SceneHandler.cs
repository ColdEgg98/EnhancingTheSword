//using TMPro;
//using UniRx;
//using UnityEngine;
//using UnityEngine.AddressableAssets;
//using UnityEngine.ResourceManagement.AsyncOperations;
//using UnityEngine.UI;

//public class SceneHandler : MonoBehaviour
//{
//    [SerializeField]
//    private Image TargetImage;
//    private string spriteID;
//    private Sprite sprite;

//    [Header("무기 이름")]
//    [SerializeField] private ReactiveProperty<TextMeshProUGUI> weaponName;

//    void Start()
//    {
//        GameManager.Instance.selectWeaponIndex
//        .Subscribe(id =>
//        {
//            spriteID = GameManager.Instance.currentData.myWeapons[id].addressID;
//            weaponName.Value.text = GameManager.Instance.currentData.myWeapons[id].name;
//            LoadSprite(spriteID);
//        })  
//        .AddTo(this);
//    }

//    public async void LoadSprite(string id)
//    {
//        AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(id);
//        sprite = await handle.Task;

//        TargetImage.sprite = sprite;
//    }
//}
