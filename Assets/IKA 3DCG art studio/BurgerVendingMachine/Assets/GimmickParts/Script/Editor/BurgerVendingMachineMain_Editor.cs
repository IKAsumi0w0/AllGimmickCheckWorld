#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(BurgerVendingMachineMain))]
public class BurgerVendingMachineMain_Editor : Editor
{
    // シリアライズドプロパティ（保持用）
    SerializedProperty _propPrefabs;   // _genPrefabs[6]
    SerializedProperty _propParent;    // _genParent
    SerializedProperty _propCounts;    // _genCounts[6]
    SerializedProperty _propBaseName;  // _genBaseName

    void OnEnable()
    {
        _propPrefabs = serializedObject.FindProperty("_genPrefabs");
        _propParent = serializedObject.FindProperty("_genParent");
        _propCounts = serializedObject.FindProperty("_genCounts");
        _propBaseName = serializedObject.FindProperty("_genBaseName");

        if (_propPrefabs.arraySize != 6)
        {
            _propPrefabs.arraySize = 6;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
        if (_propCounts.arraySize != 6)
        {
            _propCounts.arraySize = 6;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    public override void OnInspectorGUI()
    {
        // まず通常インスペクタ
        DrawDefaultInspector();

        EditorGUILayout.Space(12);
        EditorGUILayout.LabelField("▼ Pools 自動生成（6種類固定）", EditorStyles.boldLabel);

        serializedObject.Update();

        // 親・名前ベース
        EditorGUILayout.PropertyField(_propParent, new GUIContent("Parent (任意)"));
        _propBaseName.stringValue = EditorGUILayout.TextField("Name Base", _propBaseName.stringValue);

        // Prefab & Count を種類ごとに
        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Prefabs & Counts (Type 0..5)", EditorStyles.miniBoldLabel);
        EditorGUI.indentLevel++;
        for (int i = 0; i < 6; i++)
        {
            EditorGUILayout.BeginHorizontal();
            SerializedProperty pfb = _propPrefabs.GetArrayElementAtIndex(i);
            SerializedProperty cnt = _propCounts.GetArrayElementAtIndex(i);

            EditorGUILayout.PropertyField(pfb, new GUIContent($"Prefab[{i}]"), true);
            GUILayout.FlexibleSpace();
            GUILayout.Label("Count", GUILayout.Width(50));
            cnt.intValue = Mathf.Max(0, EditorGUILayout.IntField(cnt.intValue, GUILayout.Width(50)));
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();

        using (new EditorGUI.DisabledScope(!HasAtLeastOnePrefab()))
        {
            if (GUILayout.Button("Generate Pools (Clear & Create)"))
            {
                BurgerVendingMachineMain t = (BurgerVendingMachineMain)target;
                GeneratePools(t);
            }
        }

        EditorGUILayout.Space(6);
        EditorGUILayout.HelpBox(
            "・種類は 0..5 の 6種固定です。\n" +
            "・各種類 i は Prefab[i] から Count[i] 個を生成し、pools{i} に格納します。\n" +
            "・Parent 未指定なら、このスクリプトの GameObject の子として生成します。\n" +
            "・設定はシーン/Prefabに保存され、再表示されます。",
            MessageType.Info);
    }

    bool HasAtLeastOnePrefab()
    {
        for (int i = 0; i < 6; i++)
        {
            SerializedProperty elem = _propPrefabs.GetArrayElementAtIndex(i);
            if (elem.objectReferenceValue != null) return true;
        }
        return false;
    }

    void ClearExistingPools(BurgerVendingMachineMain t)
    {
        // 既存の pools0..5 を全部破棄
        DestroyArray(t.pools0);
        DestroyArray(t.pools1);
        DestroyArray(t.pools2);
        DestroyArray(t.pools3);
        DestroyArray(t.pools4);
        DestroyArray(t.pools5);

        t.pools0 = null;
        t.pools1 = null;
        t.pools2 = null;
        t.pools3 = null;
        t.pools4 = null;
        t.pools5 = null;
    }

    void DestroyArray(BurgerVendingPickupSub[] arr)
    {
        if (arr == null) return;
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] != null)
                Undo.DestroyObjectImmediate(arr[i].gameObject);
        }
    }

    void GeneratePools(BurgerVendingMachineMain t)
    {
        serializedObject.Update();

        Transform parent = (Transform)_propParent.objectReferenceValue;
        if (parent == null) parent = t.transform;
        string baseName = string.IsNullOrEmpty(_propBaseName.stringValue) ? "BurgerSub" : _propBaseName.stringValue;

        // 既存削除
        ClearExistingPools(t);
        Undo.RecordObject(t, "[Pools] Generate");

        // 6種類分まとめて作る
        t.pools0 = CreateOneType(t, parent, baseName, 0);
        t.pools1 = CreateOneType(t, parent, baseName, 1);
        t.pools2 = CreateOneType(t, parent, baseName, 2);
        t.pools3 = CreateOneType(t, parent, baseName, 3);
        t.pools4 = CreateOneType(t, parent, baseName, 4);
        t.pools5 = CreateOneType(t, parent, baseName, 5);

        EditorUtility.SetDirty(t);
        EditorSceneManager.MarkSceneDirty(t.gameObject.scene);
        serializedObject.ApplyModifiedProperties();

        Debug.Log("[Pools] Generated (old deleted) for 6 types.");
    }

    BurgerVendingPickupSub[] CreateOneType(BurgerVendingMachineMain t, Transform parent, string baseName, int typeIndex)
    {
        SerializedProperty elemPrefab = _propPrefabs.GetArrayElementAtIndex(typeIndex);
        SerializedProperty elemCount = _propCounts.GetArrayElementAtIndex(typeIndex);

        BurgerVendingPickupSub prefab = (BurgerVendingPickupSub)elemPrefab.objectReferenceValue;
        int count = Mathf.Max(0, elemCount.intValue);

        if (prefab == null || count <= 0)
        {
            if (prefab == null) Debug.LogWarningFormat(t, "[Pools] Prefab[{0}] が未設定のためスキップします。", typeIndex);
            return new BurgerVendingPickupSub[0];
        }

        BurgerVendingPickupSub[] result = new BurgerVendingPickupSub[count];

        for (int subIndex = 0; subIndex < count; subIndex++)
        {
            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(prefab.gameObject, t.gameObject.scene);
            if (go == null) continue;

            Undo.RegisterCreatedObjectUndo(go, "[Pools] Create Sub");

            go.transform.SetParent(parent, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            go.name = string.Format("{0}_{1:00}_{2:00}", baseName, typeIndex, subIndex);

            BurgerVendingPickupSub sub = go.GetComponent<BurgerVendingPickupSub>();
            if (sub == null) sub = go.AddComponent<BurgerVendingPickupSub>();

            result[subIndex] = sub;
        }

        return result;
    }
}
#endif
