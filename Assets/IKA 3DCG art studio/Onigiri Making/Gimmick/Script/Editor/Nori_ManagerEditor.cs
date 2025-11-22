using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Nori_Manager))]
public class Nori_ManagerEditor : Editor
{
    int numberOfCopies = 20;

    public override void OnInspectorGUI()
    {
        // 先に通常インスペクタ（_pool と _prefab をここで設定）
        DrawDefaultInspector();

        var script = (Nori_Manager)target;

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Nori Pool Generator (Prefab)", EditorStyles.boldLabel);
        numberOfCopies = EditorGUILayout.IntSlider("生成数", numberOfCopies, 1, 200);

        // Prefab 参照を取得
        var so = new SerializedObject(script);
        var pPrefab = so.FindProperty("_prefab");
        so.ApplyModifiedProperties();

        var ready = true;
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

    static void GenerateFromPrefab(Nori_Manager script, GameObject prefab, int count)
    {
        var parent = script._pool;
        if (parent == null || prefab == null)
        {
            Debug.LogError("[Nori_ManagerEditor] pool または prefab が未設定です。");
            return;
        }

        Undo.IncrementCurrentGroup();
        var group = Undo.GetCurrentGroup();

        // 既存子を全削除
        for (var i = parent.childCount - 1; i >= 0; i--)
            Undo.DestroyObjectImmediate(parent.GetChild(i).gameObject);

        // 生成
        var list = new System.Collections.Generic.List<Nori_Pickup>(count);
        for (var i = 0; i < count; i++)
        {
            var clone = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
            if (clone == null) clone = Object.Instantiate(prefab, parent);

            Undo.RegisterCreatedObjectUndo(clone, "Create Nori");
            clone.name = prefab.name + "_Copy_" + (i + 1);

            var pickup = clone.GetComponent<Nori_Pickup>();
            if (pickup != null) list.Add(pickup);
            else Debug.LogWarning($"{clone.name} に Nori_Pickup が見つかりません。");
        }

        // _objs に反映
        Undo.RecordObject(script, "Assign _objs");
        script._objs = list.ToArray();
        EditorUtility.SetDirty(script);

        Undo.CollapseUndoOperations(group);

        Debug.Log($"Nori を {list.Count} 個生成し、_objs に割り当てました。");
    }
}
