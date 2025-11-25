using System.IO;
using UnityEngine;

public class FirstPlayChecker : MonoBehaviour
{
    private string path;

    void Start()
    {
        path = GameManager.Instance.saveDataManager.GetIndexPath();
        GameManager.Instance.saveDataManager.wrapperPreviewData.slots = new PreviewData[3];

        if (!File.Exists(path))
        {
            for (int i = 0; i < 3; i++)
            {
                GameManager.Instance.saveDataManager.wrapperPreviewData.slots[i] = new PreviewData();
            }
            return;
        }
        else
        {
            AssignMetaData();
        }
    }

    /// <summary>MetaData의 데이터를 GameManager~.slots에 할당합니다.</summary>
    private void AssignMetaData()
    {
        string jsonMetaStr = File.ReadAllText(path);
        GameManager.Instance.saveDataManager.wrapperPreviewData = JsonUtility.FromJson<WrapperPreviewData>(jsonMetaStr);
        Debug.Log("AssignMetaData is Run");
    }
}
