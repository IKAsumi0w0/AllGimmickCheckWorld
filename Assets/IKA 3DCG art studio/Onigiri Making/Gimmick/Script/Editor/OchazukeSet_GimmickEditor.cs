using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OchazukeSet_Gimmick))]
public class OchazukeSet_GimmickEditor : Editor
{
    int numberOfCopies = 20;

    public override void OnInspectorGUI()
    {
        // まず通常のインスペクタ（_prefab と _pool をここで設定）
        DrawDefaultInspector();

        var script = (OchazukeSet_Gimmick)target;

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Ochazuke Pool Generator (Prefab)", EditorStyles.boldLabel);
        numberOfCopies = EditorGUILayout.IntSlider("生成数", numberOfCopies, 1, 200);

        // Prefab参照を取得
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

    static void GenerateFromPrefab(OchazukeSet_Gimmick script, GameObject prefab, int count)
    {
        var parent = script._pool;
        if (parent == null || prefab == null)
        {
            Debug.LogError("[OchazukeSet_GimmickEditor] pool または prefab が未設定です。");
            return;
        }

        Undo.IncrementCurrentGroup();
        var group = Undo.GetCurrentGroup();

        // 既存子オブジェクトを全削除
        for (int i = parent.childCount - 1; i >= 0; i--)
            Undo.DestroyObjectImmediate(parent.GetChild(i).gameObject);

        // 指定数生成
        var list = new System.Collections.Generic.List<Ochazuke_Pickup>(count);
        for (int i = 0; i < count; i++)
        {
            GameObject clone = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
            if (clone == null) clone = Object.Instantiate(prefab, parent);

            Undo.RegisterCreatedObjectUndo(clone, "Create Ochazuke");
            clone.name = prefab.name + "_Copy_" + (i + 1);

            var pickup = clone.GetComponent<Ochazuke_Pickup>();
            if (pickup != null) list.Add(pickup);
            else Debug.LogWarning($"{clone.name} に Ochazuke_Pickup が見つかりません。");
        }

        // _objs に反映
        Undo.RecordObject(script, "Assign _objs");
        script._objs = list.ToArray();
        EditorUtility.SetDirty(script);

        Undo.CollapseUndoOperations(group);

        Debug.Log($"Ochazuke を {list.Count} 個生成し、_objs に割り当てました。");
    }
}
