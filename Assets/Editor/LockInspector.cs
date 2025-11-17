using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class InspectorLockToggle : EditorWindow
{
    [MenuItem("Window/Toggle Inspector Lock %#l")] // Ctrl+Shift+L でアクセス
    private static void ToggleLock()
    {
        // アクティブなインスペクターウィンドウを取得
        var inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        var inspectorWindow = EditorWindow.GetWindow(inspectorType);

        // ロック状態のプロパティを取得し、現在の値を反転させる
        var isLockedProp = inspectorType.GetProperty("isLocked", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        if (isLockedProp != null)
        {
            bool isLocked = (bool)isLockedProp.GetValue(inspectorWindow, null);
            isLockedProp.SetValue(inspectorWindow, !isLocked, null);
            inspectorWindow.Repaint();
        }
    }

    // ショートカットキーの登録
    [Shortcut("Toggle Inspector Lock", KeyCode.L, ShortcutModifiers.Shift | ShortcutModifiers.Action)]
    private static void ShortcutAction()
    {
        ToggleLock();
    }
}
