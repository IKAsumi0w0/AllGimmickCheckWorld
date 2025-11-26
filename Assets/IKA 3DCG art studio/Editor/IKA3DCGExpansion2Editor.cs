using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

#if VRC_SDK_VRCSDK3
using VRC.SDKBase;          // VRC_Pickup
using VRC.SDK3.Components;  // VRCObjectSync, VRCSpatialAudioSource(ワールド用)
using VRC.Udon;             // UdonBehaviour
#endif

public class IKA3DCGExpansion2Editor : EditorWindow
{
    // ウィンドウ全体スクロール
    Vector2 scroll;

    // 任意コンポーネント検索
    MonoScript targetScript;

    // 検索結果
    List<GameObject> resultObjects = new List<GameObject>();

    // Pickup Version フィルタ種別
    enum PickupVersionFilter
    {
        All,
        Version_1_0,
        Version_1_1
    }

    PickupVersionFilter pickupVersionFilter = PickupVersionFilter.All;

#if VRC_SDK_VRCSDK3
    // AudioSource検索時に「VRCSpatialAudioSource が付いているものを除外するか」
    bool excludeVRCSpatialAudio = false;
#endif

    [MenuItem("IKA3DCG/IKA3DExp2Editor")]
    static void Open()
    {
        GetWindow<IKA3DCGExpansion2Editor>("IKA3DExp2Editor");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("IKA3DCGExpansion2Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 🔽 ウィンドウ全体をスクロール可能にする
        scroll = EditorGUILayout.BeginScrollView(scroll);

        // 任意コンポーネント検索
        DrawGenericComponentSearchGUI();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // VRChatコンポーネント検索
        DrawVRChatSearchGUI();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // Pickup Version 検索＆アップグレード
        DrawVRCPickupVersionSearchGUI();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // AudioSource 検索
        DrawAudioSourceSearchGUI();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // 検索結果
        DrawSearchResultGUI();

        EditorGUILayout.EndScrollView();
    }

    // ============================================================
    // ■ 任意コンポーネント検索 GUI
    // ============================================================
    void DrawGenericComponentSearchGUI()
    {
        EditorGUILayout.LabelField("■ 任意コンポーネント検索", EditorStyles.boldLabel);

        targetScript = (MonoScript)EditorGUILayout.ObjectField(
            "コンポーネント Script",
            targetScript,
            typeof(MonoScript),
            false
        );

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("検索"))
        {
            SearchByScript();
        }
        if (GUILayout.Button("クリア"))
        {
            resultObjects.Clear();
        }
        EditorGUILayout.EndHorizontal();
    }

    void SearchByScript()
    {
        resultObjects.Clear();

        if (targetScript == null)
        {
            Debug.LogWarning("コンポーネントの Script を指定してください。");
            return;
        }

        var type = targetScript.GetClass();
        if (type == null || !typeof(Component).IsAssignableFrom(type))
        {
            Debug.LogWarning("Component を継承した Script を指定してください。");
            return;
        }

        SearchByType(type);
    }

    // ============================================================
    // ■ VRChat コンポーネント検索
    // ============================================================
    void DrawVRChatSearchGUI()
    {
        EditorGUILayout.LabelField("■ VRChat コンポーネントクイック検索", EditorStyles.boldLabel);

#if VRC_SDK_VRCSDK3

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("VRC_Pickup"))
        {
            SearchByType(typeof(VRC_Pickup));
        }
        if (GUILayout.Button("VRCObjectSync"))
        {
            SearchByType(typeof(VRCObjectSync));
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("UdonBehaviour"))
        {
            SearchByType(typeof(UdonBehaviour));
        }

#else
        EditorGUILayout.HelpBox("VRC SDK3 が導入されていません。", MessageType.Warning);
#endif
    }

    // ============================================================
    // ■ Type 一般検索
    // ============================================================
    void SearchByType(System.Type type)
    {
        resultObjects.Clear();

        // 非アクティブも含めて検索
        Object[] objs = FindObjectsOfType(type, true);

        foreach (var o in objs)
        {
            Component c = o as Component;
            if (c != null && c.gameObject != null)
            {
                resultObjects.Add(c.gameObject);
            }
        }

        Debug.Log(type.Name + " を持つオブジェクト: " + resultObjects.Count + " 件");
    }

    // ============================================================
    // ■ VRC_Pickup Version 1.0 / 1.1 検索 & 一括アップグレード
    // ============================================================
    void DrawVRCPickupVersionSearchGUI()
    {
        EditorGUILayout.LabelField("■ VRC_Pickup Version検索 (1.0 / 1.1)", EditorStyles.boldLabel);

#if VRC_SDK_VRCSDK3

        pickupVersionFilter =
            (PickupVersionFilter)EditorGUILayout.EnumPopup("検索する Version", pickupVersionFilter);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Pickup Version で検索"))
        {
            SearchPickupByVersion();
        }
        if (GUILayout.Button("Version 1.0 を 1.1 に一括アップグレード"))
        {
            UpgradePickupVersionTo11();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(
            "VRC_Pickup 内の Enum を全スキャンし、'Version 1.0' / 'Version 1.1' を含む Enum を自動検出します。\n" +
            "一括アップグレードは Version 1.0 の Pickup だけを 1.1 に変更します（Undo で戻せます）。",
            MessageType.Info);

#else
        EditorGUILayout.HelpBox("VRC SDK3 が導入されていません。", MessageType.Warning);
#endif
    }

    void SearchPickupByVersion()
    {
        resultObjects.Clear();

#if VRC_SDK_VRCSDK3

        VRC_Pickup[] pickups = FindObjectsOfType<VRC_Pickup>(true);

        foreach (var p in pickups)
        {
            if (p == null || p.gameObject == null) continue;

            SerializedObject so = new SerializedObject(p);

            // ---- Version 用 Enum プロパティを自動探索 ----
            SerializedProperty versionProp = FindPickupVersionProperty(so);
            if (versionProp == null)
            {
                if (pickupVersionFilter == PickupVersionFilter.All)
                    resultObjects.Add(p.gameObject);

                continue;
            }

            int idx = versionProp.enumValueIndex;
            string currentLabel = "";
            if (idx >= 0 && idx < versionProp.enumDisplayNames.Length)
            {
                currentLabel = versionProp.enumDisplayNames[idx];
            }

            bool match = false;

            switch (pickupVersionFilter)
            {
                case PickupVersionFilter.All:
                    match = true;
                    break;

                case PickupVersionFilter.Version_1_0:
                    if (currentLabel.Contains("1.0")) match = true;
                    break;

                case PickupVersionFilter.Version_1_1:
                    if (currentLabel.Contains("1.1")) match = true;
                    break;
            }

            if (match)
            {
                resultObjects.Add(p.gameObject);
            }
        }

        Debug.Log("Pickup Version 検索: " + resultObjects.Count + " 件ヒット");

#endif
    }

    // ★ ここが一括アップグレード処理
    void UpgradePickupVersionTo11()
    {
        resultObjects.Clear();

#if VRC_SDK_VRCSDK3

        int upgradedCount = 0;
        VRC_Pickup[] pickups = FindObjectsOfType<VRC_Pickup>(true);

        foreach (var p in pickups)
        {
            if (p == null || p.gameObject == null) continue;

            SerializedObject so = new SerializedObject(p);
            SerializedProperty versionProp = FindPickupVersionProperty(so);
            if (versionProp == null)
                continue;

            string[] names = versionProp.enumDisplayNames;
            int index10 = -1;
            int index11 = -1;

            for (int i = 0; i < names.Length; i++)
            {
                string n = names[i];
                if (string.IsNullOrEmpty(n)) continue;

                if (n.Contains("Version 1.0") || n.Contains("1.0"))
                    index10 = i;
                if (n.Contains("Version 1.1") || n.Contains("1.1"))
                    index11 = i;
            }

            if (index11 < 0) // 1.1 が見つからない場合は何もしない
                continue;

            int currentIdx = versionProp.enumValueIndex;

            // 今 1.0 のものだけ 1.1 に変更
            if (index10 >= 0 && currentIdx == index10)
            {
                Undo.RecordObject(p, "Upgrade VRC_Pickup Version to 1.1");

                versionProp.enumValueIndex = index11;
                so.ApplyModifiedProperties();

                upgradedCount++;
                resultObjects.Add(p.gameObject);
            }
        }

        Debug.Log("VRC_Pickup Version 1.0 → 1.1 へアップグレード: " + upgradedCount + " 個変更しました。");

#endif
    }

    // VRC_Pickup の SerializedObject から、Version 用 Enum プロパティを探す共通関数
    SerializedProperty FindPickupVersionProperty(SerializedObject so)
    {
        SerializedProperty it = so.GetIterator();
        bool enterChildren = true;

        while (it.NextVisible(enterChildren))
        {
            enterChildren = false;

            if (it.propertyType != SerializedPropertyType.Enum)
                continue;

            string[] names = it.enumDisplayNames;
            bool isVersionEnum = false;

            foreach (var n in names)
            {
                if (string.IsNullOrEmpty(n)) continue;
                if (n.Contains("Version 1.0") || n.Contains("Version 1.1"))
                {
                    isVersionEnum = true;
                    break;
                }
            }

            if (isVersionEnum)
            {
                return it.Copy();
            }
        }

        return null;
    }

    // ============================================================
    // ■ AudioSource 検索
    // ============================================================
    void DrawAudioSourceSearchGUI()
    {
        EditorGUILayout.LabelField("■ AudioSource 検索", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "現在開いているシーン内の AudioSource が付いた GameObject を検索します。",
            MessageType.Info);

#if VRC_SDK_VRCSDK3
        // 「VRCSpatialAudioSource を持っていないものだけ」に絞るオプション
        excludeVRCSpatialAudio = EditorGUILayout.ToggleLeft(
            "VRC Spatial Audio Source が付いていないオブジェクトだけに絞り込む",
            excludeVRCSpatialAudio);
#else
        EditorGUILayout.HelpBox(
            "VRC SDK3 がないため、VRC Spatial Audio Source の有無による絞り込みは無効です。",
            MessageType.Info);
#endif

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("AudioSource を検索"))
        {
            SearchAudioSourceObjects();
        }
        if (GUILayout.Button("クリア"))
        {
            resultObjects.Clear();
        }
        EditorGUILayout.EndHorizontal();
    }

    void SearchAudioSourceObjects()
    {
        resultObjects.Clear();

        AudioSource[] sources = FindObjectsOfType<AudioSource>(true);
        foreach (var s in sources)
        {
            if (s == null) continue;

            GameObject go = s.gameObject;
            if (go == null) continue;

#if VRC_SDK_VRCSDK3
            // VRCSpatialAudioSource を持っているオブジェクトを除外する場合
            if (excludeVRCSpatialAudio && go.GetComponent<VRCSpatialAudioSource>() != null)
            {
                continue;
            }
#endif
            resultObjects.Add(go);
        }

        Debug.Log("AudioSource を持つオブジェクト: " + resultObjects.Count + " 件");
    }

    // ============================================================
    // ■ 検索結果 GUI
    // ============================================================
    void DrawSearchResultGUI()
    {
        EditorGUILayout.LabelField("■ 検索結果", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("ヒット数: " + resultObjects.Count);

        if (resultObjects.Count > 0)
        {
            if (GUILayout.Button("すべて選択"))
            {
                Selection.objects = resultObjects.ToArray();
            }
        }

        EditorGUILayout.Space();

        foreach (var go in resultObjects)
        {
            if (go == null) continue;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(go.name, GUILayout.MinWidth(150)))
            {
                SelectObject(go);
            }

            EditorGUILayout.ObjectField(go, typeof(GameObject), true);

            if (GUILayout.Button("選択", GUILayout.Width(50)))
            {
                SelectObject(go);
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    void SelectObject(GameObject go)
    {
        Selection.activeGameObject = go;
        EditorGUIUtility.PingObject(go);
    }
}
