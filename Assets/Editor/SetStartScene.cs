using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class SetStartScene
{
    static SetStartScene()
    {
        // 플레이 버튼을 눌렀을 때 시작할 씬의 경로를 지정합니다.
        // Assets 폴더부터의 경로를 적어주세요.
        string scenePath = "Assets/01. Scenes/StartScene.unity"; 
        
        SceneAsset sceneObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

        if (sceneObject != null)
        {
            EditorSceneManager.playModeStartScene = sceneObject;
        }
        else
        {
            Debug.LogWarning($"{scenePath} 경로에서 씬을 찾을 수 없습니다.");
        }
    }
}
