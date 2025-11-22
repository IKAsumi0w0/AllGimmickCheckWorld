using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Teacup_Manager))]
public class Teacup_ManagerEditor : Editor
{
    int numberOfCopies = 20;

    public override void OnInspectorGUI()
    {
        // まず通常のインスペクタ（_prefab と _pool をここで設定）
        DrawDefaultInspector();

        var script = (Teacup_Manager)target;

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Teacup Pool Generator (Prefab)", EditorStyles.boldLabel);
        numberOfCopies = EditorGUILayout.IntSlider("生成数", numberOfCopies, 1, 200);

        // 入力チェック
        var so = new SerializedObject(script);
        var pPrefab = so.FindProperty("_prefab");
        so.ApplyModifiedProperties();

        bool ready = true;
        if (script._pool == null)
        {
            EditorGUILayout.HelpBox("_pool が未設定です。", MessageType.Warning);
            ready = false;
        }
        if (pPrefab == null || pPrefab.objectReferenceValue == null)
        {
            EditorGUILayout.HelpBox("_prefab（生成元 Prefab）が未設定です。", MessageType.Warning);
            ready = false;
        }

        GUI.enabled = ready;
        if (GUILayout.Button("Prefab から生成して _objs に割り当て"))
        {
            GenerateFromPrefab(script, (GameObject)pPrefab.objectReferenceValue, numberOfCopies);
        }
        GUI.enabled = true;
    }

    static void GenerateFromPrefab(Teacup_Manager script, GameObject prefab, int count)
    {
        var parent = script._pool;
        if (parent == null || prefab == null)
        {
            Debug.LogError("[Teacup_ManagerEditor] pool または prefab が未設定です。");
            return;
        }

        Undo.IncrementCurrentGroup();
        var group = Undo.GetCurrentGroup();

        // 既存子を全削除
        for (int i = parent.childCount - 1; i >= 0; i--)
            Undo.DestroyObjectImmediate(parent.GetChild(i).gameObject);

        // 生成
        var list = new System.Collections.Generic.List<Teacup_Pickup>(count);
        for (int i = 0; i < count; i++)
        {
            GameObject clone = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
            if (clone == null) clone = Object.Instantiate(prefab, parent);

            Undo.RegisterCreatedObjectUndo(clone, "Create Teacup");
            clone.name = prefab.name + "_Copy_" + (i + 1);

            var sub = clone.GetComponent<Teacup_Pickup>();
            if (sub != null) list.Add(sub);
            else Debug.LogWarning($"{clone.name} に Teacup_Pickup が見つかりません。");
        }

        // _objs に反映
        Undo.RecordObject(script, "Assign _objs");
        script._objs = list.ToArray();
        EditorUtility.SetDirty(script);

        Undo.CollapseUndoOperations(group);

        Debug.Log($"Teacup を {list.Count} 個生成し、_objs に割り当てました。");
    }
}
