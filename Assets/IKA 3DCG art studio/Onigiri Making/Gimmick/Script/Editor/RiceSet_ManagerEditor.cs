using UnityEditor;
using UnityEngine;

// RiceSet_Manager のインスペクタに、Bamboo / RicePaddle / ShapedRiceBall の生成UIを載せる
[CustomEditor(typeof(RiceSet_Manager))]
[CanEditMultipleObjects]
public class RiceSet_ManagerEditor : Editor
{
    int numberOfCopies_Bamboo = 20;
    int numberOfCopies_RicePaddle = 20;
    int numberOfCopies_RiceBall = 20;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 通常のインスペクタ描画
        DrawDefaultInspector();

        var rs = (RiceSet_Manager)target;

        // ===== Bamboo Leaf Plate =====
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Bamboo Leaf Plate (from RiceSet)", EditorStyles.boldLabel);

        bool bambooReady = true;
        if (rs._bpm == null) { EditorGUILayout.HelpBox("_bpm (BambooLeafPlate_Manager) を割り当ててください。", MessageType.Warning); bambooReady = false; }
        else if (rs._bpm._pool == null) { EditorGUILayout.HelpBox("_bpm の Pool が未設定です。", MessageType.Warning); bambooReady = false; }
        if (rs._bpmPrefab == null) { EditorGUILayout.HelpBox("_bpmPrefab (生成元Prefab) が未設定です。", MessageType.Warning); bambooReady = false; }
        numberOfCopies_Bamboo = EditorGUILayout.IntSlider("複製する数 (Bamboo)", numberOfCopies_Bamboo, 1, 200);

        // ===== Rice Paddle =====
        EditorGUILayout.Space(14);
        EditorGUILayout.LabelField("Rice Paddle (from RiceSet)", EditorStyles.boldLabel);

        bool paddleReady = true;
        if (rs._rpm == null) { EditorGUILayout.HelpBox("_rpm (RicePaddle_Manager) を割り当ててください。", MessageType.Warning); paddleReady = false; }
        else if (rs._rpm._pool == null) { EditorGUILayout.HelpBox("_rpm の Pool が未設定です。", MessageType.Warning); paddleReady = false; }
        if (rs._rpmPrefab == null) { EditorGUILayout.HelpBox("_rpmPrefab (生成元Prefab) が未設定です。", MessageType.Warning); paddleReady = false; }
        numberOfCopies_RicePaddle = EditorGUILayout.IntSlider("複製する数 (RicePaddle)", numberOfCopies_RicePaddle, 1, 200);

        // ===== Shaped Rice Ball =====
        EditorGUILayout.Space(14);
        EditorGUILayout.LabelField("Shaped Rice Ball (from RiceSet)", EditorStyles.boldLabel);

        bool riceBallReady = true;
        if (rs._rbm == null) { EditorGUILayout.HelpBox("_rbm (ShapedRiceBall_Manager) を割り当ててください。", MessageType.Warning); riceBallReady = false; }
        else if (rs._rbm._pool == null) { EditorGUILayout.HelpBox("_rbm の Pool が未設定です。", MessageType.Warning); riceBallReady = false; }
        if (rs._rbmPrefab == null) { EditorGUILayout.HelpBox("_rbmPrefab (生成元Prefab) が未設定です。", MessageType.Warning); riceBallReady = false; }
        numberOfCopies_RiceBall = EditorGUILayout.IntSlider("複製する数 (RiceBall)", numberOfCopies_RiceBall, 1, 200);

        EditorGUILayout.Space(16);
        GUI.enabled = bambooReady || paddleReady || riceBallReady;
        if (GUILayout.Button("↑ 上記設定で一括生成・割り当て"))
        {
            int createdB = 0, createdP = 0, createdR = 0;

            Undo.IncrementCurrentGroup();
            var group = Undo.GetCurrentGroup();

            if (bambooReady)
                createdB = GenerateAndAssign_Bamboo(rs, rs._bpm, rs._bpmPrefab, numberOfCopies_Bamboo);

            if (paddleReady)
                createdP = GenerateAndAssign_RicePaddle(rs, rs._rpm, rs._rpmPrefab, numberOfCopies_RicePaddle);

            if (riceBallReady)
                createdR = GenerateAndAssign_RiceBall(rs, rs._rbm, rs._rbmPrefab, numberOfCopies_RiceBall);

            Undo.CollapseUndoOperations(group);

            Debug.Log($"一括生成 完了: Bamboo={createdB}, RicePaddle={createdP}, RiceBall={createdR}");
        }
        GUI.enabled = true;

        serializedObject.ApplyModifiedProperties();
    }

    // ---------- Bamboo ----------
    // 返り値: 生成した個数
    static int GenerateAndAssign_Bamboo(RiceSet_Manager rs, BambooLeafPlate_Manager bpm, GameObject prefab, int count)
    {
        var parent = bpm._pool;
        if (parent == null || prefab == null) { Debug.LogError("Pool または Prefab が未指定です。(Bamboo)"); return 0; }

        // 既存子 全削除
        for (int i = parent.childCount - 1; i >= 0; i--)
            Undo.DestroyObjectImmediate(parent.GetChild(i).gameObject);

        // 生成
        var list = new System.Collections.Generic.List<BambooLeafPlate_Pickup>(count);
        for (int i = 0; i < count; i++)
        {
            GameObject clone = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
            if (clone == null) clone = Object.Instantiate(prefab, parent);
            Undo.RegisterCreatedObjectUndo(clone, "Create BambooLeafPlate Object");
            clone.name = prefab.name + "_Copy_" + (i + 1);

            var pick = clone.GetComponent<BambooLeafPlate_Pickup>();
            if (pick != null) list.Add(pick);

            // ★ Gimmick に RiceSet_Manager をセット（存在する場合のみ）
            // 皿側は Pickup の _main が BambooLeafPlate_Gimmick
            var gimmick = pick != null ? pick._main : null;
            if (gimmick != null)
            {
                var so = new SerializedObject(gimmick);
                so.Update();
                var pSetM = so.FindProperty("_setM"); // フィールドが追加されているケース
                if (pSetM != null) pSetM.objectReferenceValue = rs;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(gimmick);
            }
        }

        // _objs へ反映
        Undo.RecordObject(bpm, "Assign _objs (Bamboo)");
        bpm._objs = list.ToArray();
        EditorUtility.SetDirty(bpm);

        // PlateNo 採番
        for (int i = 0; i < bpm._objs.Length; i++)
        {
            var gimmick = bpm._objs[i]?._main; // BambooLeafPlate_Gimmick
            if (gimmick == null) continue;

            var so = new SerializedObject(gimmick);
            so.Update();
            var prop = so.FindProperty("_plateNo");
            if (prop != null) prop.intValue = i;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(gimmick);
        }

        Debug.Log($"{list.Count} 個の BambooLeafPlate を生成・割り当て・PlateNo採番・_setM設定(存在時) 完了。");
        return list.Count;
    }

    // ---------- RicePaddle ----------
    static int GenerateAndAssign_RicePaddle(RiceSet_Manager rs, RicePaddle_Manager rpm, GameObject prefab, int count)
    {
        var parent = rpm._pool;
        if (parent == null || prefab == null) { Debug.LogError("Pool または Prefab が未指定です。(RicePaddle)"); return 0; }

        // 既存子 全削除
        for (int i = parent.childCount - 1; i >= 0; i--)
            Undo.DestroyObjectImmediate(parent.GetChild(i).gameObject);

        // 生成
        var list = new System.Collections.Generic.List<RicePaddle_Pickup>(count);
        for (int i = 0; i < count; i++)
        {
            GameObject clone = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
            if (clone == null) clone = Object.Instantiate(prefab, parent);
            Undo.RegisterCreatedObjectUndo(clone, "Create RicePaddle Object");
            clone.name = prefab.name + "_Copy_" + (i + 1);

            var pick = clone.GetComponent<RicePaddle_Pickup>();
            if (pick != null) list.Add(pick);
            else Debug.LogWarning($"[Paddle] {clone.name} に RicePaddle_Pickup が見つかりません。");
        }

        // _objs へ反映
        Undo.RecordObject(rpm, "Assign _objs (RicePaddle)");
        rpm._objs = list.ToArray();
        EditorUtility.SetDirty(rpm);

        // ★ Gimmick に RiceSet と _paddleNo を設定
        for (int i = 0; i < rpm._objs.Length; i++)
        {
            var gimmick = rpm._objs[i]?._main; // RicePaddle_Gimmick
            if (gimmick == null) continue;

            var so = new SerializedObject(gimmick);
            so.Update();

            var pSetM = so.FindProperty("_setM");
            if (pSetM != null) pSetM.objectReferenceValue = rs;

            var pNo = so.FindProperty("_paddleNo");
            if (pNo != null) pNo.intValue = i;

            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(gimmick);
        }

        Debug.Log($"{list.Count} 個の RicePaddle を生成・割り当て・_setM設定・_paddleNo採番 完了。");
        return list.Count;
    }

    // ---------- ShapedRiceBall ----------
    static int GenerateAndAssign_RiceBall(RiceSet_Manager rs, ShapedRiceBall_Manager rbm, GameObject prefab, int count)
    {
        var parent = rbm._pool;
        if (rs == null || parent == null || prefab == null) { Debug.LogError("RiceSet / Pool / Prefab のいずれかが未指定です。(RiceBall)"); return 0; }

        // 既存子 全削除
        for (int i = parent.childCount - 1; i >= 0; i--)
            Undo.DestroyObjectImmediate(parent.GetChild(i).gameObject);

        // 生成
        var list = new System.Collections.Generic.List<ShapedRiceBall_Pickup>(count);
        for (int i = 0; i < count; i++)
        {
            GameObject clone = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
            if (clone == null) clone = Object.Instantiate(prefab, parent);
            Undo.RegisterCreatedObjectUndo(clone, "Create ShapedRiceBall Object");
            clone.name = prefab.name + "_Copy_" + (i + 1);

            var pick = clone.GetComponent<ShapedRiceBall_Pickup>();
            if (pick != null) list.Add(pick);
        }

        // _objs へ反映
        Undo.RecordObject(rbm, "Assign _objs (RiceBall)");
        rbm._objs = list.ToArray();
        EditorUtility.SetDirty(rbm);

        // ★ Gimmick に _setM と確率を設定
        int exp = rs._explosionProbability; // プロパティでなくフィールドを参照（ご提示コード準拠）
        int roll = rs._rollProbability;

        for (int i = 0; i < rbm._objs.Length; i++)
        {
            var gimmick = rbm._objs[i]?._main; // ShapedRiceBall_Gimmick
            if (gimmick == null) continue;

            Undo.RecordObject(gimmick, "Set RiceSet & Probabilities");
            gimmick._setM = rs;
            gimmick._explosionProbability = exp;
            gimmick._rollProbability = roll;
            EditorUtility.SetDirty(gimmick);
        }

        Debug.Log($"{list.Count} 個の ShapedRiceBall を生成・割り当て・_setM設定・確率反映 完了。");
        return list.Count;
    }
}
