using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class ToggleActiveShortcut
{
    // Ctrl+H 로 토글
    [MenuItem("GameObject/Toggle Active %h", false, 0)]
    private static void ToggleActive()
    {
        var selected = Selection.gameObjects;
        if (selected == null || selected.Length == 0)
        {
            Debug.LogWarning("No GameObject selected.");
            return;
        }

        // 동일 상태로 맞추거나 단순 반전 중 선택: 여기선 '각자 반전'
        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();

        foreach (var obj in selected)
        {
            // Prefab 단계에서 SetActive는 허용되지만, Prefab 에셋 자체 수정은 불가
            Undo.RecordObject(obj, "Toggle Active");
            obj.SetActive(!obj.activeSelf);
            EditorUtility.SetDirty(obj);
        }

        Undo.CollapseUndoOperations(group);
    }
}

public static class InspectorLockToggle
{
    [MenuItem("Window/Toggle Inspector Lock #l")] // Shift+L
    private static void ToggleInspectorLock()
    {
        // 현재 열려 있는 Inspector 창 가져오기
        var inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        var window = EditorWindow.GetWindow(inspectorType);

        // isLocked 속성 가져오기
        var isLockedProp = inspectorType.GetProperty("isLocked", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        bool current = (bool)isLockedProp.GetValue(window, null);
        isLockedProp.SetValue(window, !current, null);

        window.Repaint();
    }
}