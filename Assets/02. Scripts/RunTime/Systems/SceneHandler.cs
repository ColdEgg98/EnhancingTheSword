using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    [SerializeField]
    private Image TargetImage;
    private string spriteID;
    private Sprite sprite;

    void Start()
    {
        // 내가 선택한 (보고있는) 무기의 addressID를 따옴
        spriteID = GameManager.Instance.currentData.myWeapons[GameManager.Instance.selectWeaponIndex].addressID;
        LoadSprite(spriteID);
    }

    public async void LoadSprite(string id)
    {
        AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(id);
        sprite = await handle.Task;

        TargetImage.sprite = sprite;
    }
}
