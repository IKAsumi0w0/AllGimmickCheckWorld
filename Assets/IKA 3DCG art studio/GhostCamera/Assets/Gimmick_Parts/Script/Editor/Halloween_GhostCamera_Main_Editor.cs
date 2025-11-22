#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Halloween_GhostCamera_Main))]
public class Halloween_GhostCamera_Main_Editor : Editor
{
    const string kGenPrefix = "[PhotoGen] ";
    int _generateCount = 8; // 既定 8

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var t = (Halloween_GhostCamera_Main)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("▼ Prefab 生成（ワンクリック一括処理）", EditorStyles.boldLabel);

        _generateCount = Mathf.Max(1, EditorGUILayout.IntField("Generate Count", _generateCount));

        using (new EditorGUI.DisabledScope(t._anchor == null || t._photoPrefab == null))
        {
            if (GUILayout.Button("Generate under _anchor (Clear → Generate → Refresh → Init)"))
            {
                ClearGenerated(t);
                Generate(t, t._photoPrefab, _generateCount);
                RefreshCardsFromChildren(t);
                InitAllCards(t);

                Debug.Log($"[GhostCamera] Generate 完了: {_generateCount}枚生成 → Refresh & Init All 実行済み。");
            }
        }
    }

    void Generate(Halloween_GhostCamera_Main t, GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, t._anchor);
            if (!go)
            {
                Debug.LogError("[GhostCamera] Prefab instantiate failed.");
                continue;
            }

            Undo.RegisterCreatedObjectUndo(go, "Generate PhotoCard");
            go.name = kGenPrefix + prefab.name + "_" + System.Guid.NewGuid().ToString("N").Substring(0, 6);

            var card = go.GetComponentInChildren<PhotoCardController>(true);
            if (card == null)
            {
                Debug.LogWarning($"[GhostCamera] '{go.name}' に PhotoCardController が見つかりません。");
                continue;
            }

            AutoAssignOnCard(card);

            // ParentConstraint の Maintain Offset セットアップ（Prefab 姿勢維持）
            if (card._constraint != null)
            {
                var srcTf = (t._ejectOrigin ? t._ejectOrigin : t._anchor);

                var src = new UnityEngine.Animations.ConstraintSource();
                src.sourceTransform = srcTf;
                src.weight = 1f;

                if (card._constraint.sourceCount == 0) card._constraint.AddSource(src);
                else card._constraint.SetSource(0, src);

                Vector3 transOffset = srcTf ? srcTf.InverseTransformPoint(card.transform.position)
                                            : card.transform.position;
                Quaternion relRot = srcTf ? Quaternion.Inverse(srcTf.rotation) * card.transform.rotation
                                          : card.transform.rotation;

                card._constraint.SetTranslationOffset(0, transOffset);
                card._constraint.SetRotationOffset(0, relRot.eulerAngles);

                // 生成時に ON（Maintain Offset 済みなので姿勢は崩れない）
                card._constraint.constraintActive = true;
            }

            // 初期は非表示＆当たり無効
            card.HideCard();

            EditorUtility.SetDirty(go);
            EditorUtility.SetDirty(card);
        }

        EditorUtility.SetDirty(t._anchor);
        Debug.Log($"[GhostCamera] Generated {count} card(s) under '{t._anchor.name}'.");
    }

    void ClearGenerated(Halloween_GhostCamera_Main t)
    {
        if (!t._anchor) return;

        var list = new System.Collections.Generic.List<GameObject>();
        foreach (Transform c in t._anchor)
        {
            if (c && c.name.StartsWith(kGenPrefix)) list.Add(c.gameObject);
        }

        foreach (var go in list) Undo.DestroyObjectImmediate(go);
        if (list.Count > 0) EditorUtility.SetDirty(t._anchor);

        Debug.Log($"[GhostCamera] Cleared {list.Count} generated card(s).");
    }

    void RefreshCardsFromChildren(Halloween_GhostCamera_Main t)
    {
        var cards = GetCards(t);
        Undo.RecordObject(t, "Assign Cards");
        t._cards = cards;
        EditorUtility.SetDirty(t);
        Debug.Log($"[GhostCamera] _cards updated: {cards.Length} entries.");
    }

    void InitAllCards(Halloween_GhostCamera_Main t)
    {
        var cards = GetCards(t);
        foreach (var c in cards)
        {
            if (!c) continue;
            Undo.RecordObject(c, "Init Card");
            c.HideCard();
            EditorUtility.SetDirty(c);
        }
        Debug.Log($"[GhostCamera] Initialized {cards.Length} cards.");
    }

    PhotoCardController[] GetCards(Halloween_GhostCamera_Main t)
    {
        if (!t._anchor) return new PhotoCardController[0];
        return t._anchor.GetComponentsInChildren<PhotoCardController>(true);
    }

    void AutoAssignOnCard(PhotoCardController c)
    {
        // --- Renderer 推定（名前ヒント → フォールバック） ---
        var rends = c.GetComponentsInChildren<Renderer>(true);

        Renderer frame = null;
        Renderer photo = null;

        if (rends != null)
        {
            for (int i = 0; i < rends.Length; i++)
            {
                var r = rends[i];
                if (!r) continue;
                string n = r.gameObject.name.ToLower();

                // フレーム候補
                if (frame == null &&
                    (n.Contains("frame") || n.Contains("border") || n.Contains("mask")))
                {
                    frame = r;
                    continue;
                }

                // 写真候補
                if (photo == null &&
                    (n.Contains("photo") || n.Contains("image") || n.Contains("picture")))
                {
                    photo = r;
                }
            }
        }

        // フォールバック：1つ目＝frame、最後＝photo
        if (frame == null && rends != null && rends.Length >= 1) frame = rends[0];
        if (photo == null && rends != null && rends.Length >= 2) photo = rends[rends.Length - 1];

        // 代入
        if (c._frameRenderer != frame) c._frameRenderer = frame;
        if (c._photoRenderer != photo) c._photoRenderer = photo;

        // _photoRoot 推定（未設定時は photo の親を優先）
        if (c._photoRoot == null)
        {
            if (photo != null && photo.transform.parent != null && photo.transform.parent.gameObject != c.gameObject)
                c._photoRoot = photo.transform.parent.gameObject;
            else if (photo != null)
                c._photoRoot = photo.gameObject;
        }

        // Collider 推定
        if (c._collider == null)
        {
            var col = c.GetComponentInChildren<Collider>(true);
            if (col != null) c._collider = col;
        }

        // Pickup 参照のみ（自動追加はしない）
        if (c._pickup == null)
        {
            var p = c.GetComponentInChildren<VRC.SDK3.Components.VRCPickup>(true);
            if (p != null) c._pickup = p;
        }

        // ParentConstraint 無ければ追加（これは許可）
        if (c._constraint == null)
        {
            var pc = c.gameObject.GetComponent<UnityEngine.Animations.ParentConstraint>();
            if (pc == null) pc = Undo.AddComponent<UnityEngine.Animations.ParentConstraint>(c.gameObject);
            c._constraint = pc;
        }
    }
}
#endif
