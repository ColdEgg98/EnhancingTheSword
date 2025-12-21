using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class ToggleActiveShortcut
{
    // Ctrl+H 단축키 할당
    [MenuItem("GameObject/Toggle Active %h", false, 0)]
    private static void ToggleActive()
    {
        var selected = Selection.gameObjects;
        if (selected == null || selected.Length == 0)
        {
            Debug.LogWarning("No GameObject selected.");
            return;
        }

        // 여러 오브젝트를 토글해도 Undo(Ctrl+Z) 한 번으로 되돌리기 위해 그룹 시작
        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();

        foreach (var obj in selected)
        {
            // 변경 사항을 Undo 시스템에 기록하고, 저장 필요(Dirty) 상태로 표시
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
        // 현재 열려 있는 Inspector 윈도우 인스턴스 가져오기
        var inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        var window = EditorWindow.GetWindow(inspectorType);

        // 비공개(private) 속성인 'isLocked' 프로퍼티 정보 가져오기
        var isLockedProp = inspectorType.GetProperty("isLocked", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        bool current = (bool)isLockedProp.GetValue(window, null);
        isLockedProp.SetValue(window, !current, null);

        window.Repaint();
    }
}