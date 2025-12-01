using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    [SerializeField]
    private Image TargetImage;
    public string spriteID;
    
    private Sprite sprite;

    void Start()
    {
        LoadSprite(spriteID);
    }

    public async void LoadSprite(string id)
    {
        AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(id);
        sprite = await handle.Task;

        TargetImage.sprite = sprite;
    }
}
