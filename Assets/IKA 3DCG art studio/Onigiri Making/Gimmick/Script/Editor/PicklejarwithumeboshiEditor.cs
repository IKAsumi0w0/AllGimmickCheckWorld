using UnityEditor;
using UnityEngine;
using UdonSharp;
using UdonSharpEditor;

[CustomEditor(typeof(PickleJarwithUmeboshiGimmick))]
public class PickleJarwithUmeboshiEditor : Editor
{
    int numberOfCopies = 20;

    public override void OnInspectorGUI()
    {
        // UdonSharp 標準ヘッダー（InteractionText / SyncMode など）
        var udon = target as UdonSharpBehaviour;
        if (udon != null)
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(udon)) return;
        }

        // ここで通常のインスペクタを描画（_objs や _pool など）
        DrawDefaultInspector();

        // ここから下がカスタム部分
        var script = (PickleJarwithUmeboshiGimmick)target;

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Umeboshi Pool Generator (Prefab)", EditorStyles.boldLabel);

        numberOfCopies = EditorGUILayout.IntSlider("生成数", numberOfCopies, 1, 200);

        bool ready = true;
        if (script._pool == null)
        {
            EditorGUILayout.HelpBox("_pool が未設定です。", MessageType.Warning);
            ready = false;
        }
        if (script._prefab == null)
        {
            EditorGUILayout.HelpBox("_prefab（生成元 Prefab）が未設定です。", MessageType.Warning);
            ready = false;
        }

        GUI.enabled = ready;
        if (GUILayout.Button("Prefab から生成して _objs に割り当て"))
        {
            GenerateFromPrefab(script, script._prefab, numberOfCopies);
        }
        GUI.enabled = true;
    }

    static void GenerateFromPrefab(PickleJarwithUmeboshiGimmick script, GameObject prefab, int count)
    {
        var parent = script._pool;
        if (parent == null || prefab == null)
        {
            Debug.LogError("[PickleJarwithUmeboshiEditor] pool または prefab が未設定です。");
            return;
        }

        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();

        // 既存の子を全削除
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Undo.DestroyObjectImmediate(parent.GetChild(i).gameObject);
        }

        var list = new System.Collections.Generic.List<Umebosshi_Pickup>(count);

        for (int i = 0; i < count; i++)
        {
            GameObject clone = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
            if (clone == null)
                clone = Object.Instantiate(prefab, parent);

            Undo.RegisterCreatedObjectUndo(clone, "Create Umeboshi");
            clone.name = prefab.name + "_Copy_" + (i + 1);

            var sub = clone.GetComponent<Umebosshi_Pickup>();
            if (sub != null)
                list.Add(sub);
            else
                Debug.LogWarning(clone.name + " に Umebosshi_Pickup が見つかりません。");
        }

        Undo.RecordObject(script, "Assign _objs");
        script._objs = list.ToArray();
        EditorUtility.SetDirty(script);

        Undo.CollapseUndoOperations(group);

        Debug.Log("Umeboshi を " + list.Count + " 個生成し、_objs に割り当てました。");
    }
}
