using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using YamlDotNet.RepresentationModel;

public class IKA3DCGExpansionEditor : EditorWindow
{
    // =========================
    // 列挙
    // =========================
    public enum Direction { X, Y, Z }
    public enum AddStringPosition { None, First, Last }
    public enum ColliderType { Box, Sphere }
    public enum SphereColliderBasis { Min, Average, Max }

    public enum GridPlane { XZ, XY, YZ }
    public enum ArrangeOrderMode { NameOrder, HierarchyOrder, SelectionOrder }

    // =========================
    // データ構造
    // =========================
    public class XYZ
    {
        public float xMin = Mathf.Infinity;
        public float xMax = Mathf.NegativeInfinity;
        public float yMin = Mathf.Infinity;
        public float yMax = Mathf.NegativeInfinity;
        public float zMin = Mathf.Infinity;
        public float zMax = Mathf.NegativeInfinity;

        public float xRange;
        public float yRange;
        public float zRange;
        public float maxRange;
        public Vector3 core;
        public Vector3 aveCore;
        public List<Vector3> vtxList = new List<Vector3>();

        public Vector3 MinVtx => new Vector3(xMin, yMin, zMin);
        public Vector3 MaxVtx => new Vector3(xMax, yMax, zMax);

        public XYZ() { }

        public XYZ(List<Vector3> list)
        {
            for (int i = 0; i < list.Count; i++) Check(list[i]);
            Calculation();
        }

        public void Calculation()
        {
            xRange = Math.Abs(xMax - xMin);
            yRange = Math.Abs(yMax - yMin);
            zRange = Math.Abs(zMax - zMin);
            core = new Vector3((xRange / 2f) + xMin, (yRange / 2f) + yMin, (zRange / 2f) + zMin);

            float[] arr = new float[] { xRange, yRange, zRange };
            Vector3 tmp = Vector3.zero;
            for (int i = 0; i < vtxList.Count; i++) tmp += vtxList[i];
            aveCore = vtxList.Count > 0 ? tmp / vtxList.Count : Vector3.zero;
            maxRange = arr.Max();
        }

        public void Check(Vector3 vtx)
        {
            if (vtx.x < xMin) xMin = vtx.x;
            if (vtx.y < yMin) yMin = vtx.y;
            if (vtx.z < zMin) zMin = vtx.z;
            if (vtx.x > xMax) xMax = vtx.x;
            if (vtx.y > yMax) yMax = vtx.y;
            if (vtx.z > zMax) zMax = vtx.z;
            vtxList.Add(vtx);
        }
    }

    // =========================
    // フィールド（英語名）
    // =========================
    // --- Common ---
    Vector2 _scroll;
    static int _selectionCount;
    List<GameObject> _recentSelectionCache = new List<GameObject>();

    // --- Collider Generation ---
    string _addName = "Pickup";
    float _colliderScale = 1f;
    AddStringPosition _addStringPos = AddStringPosition.None;
    ColliderType _colliderType = ColliderType.Box;
    SphereColliderBasis _sphereBasis = SphereColliderBasis.Average;

    // --- VRChat (Auto-detect with/without Udon) ---
    bool _addVRCPickup = false;
    bool _pickupAutoHold = false;
    bool _rigidUseGravity = true;
    bool _rigidIsKinematic = false;
    bool _addObjectSync = false;

    // --- Fit Existing Collider ---
    float _fitPadding = 0f;      // ローカル各軸に±付与
    float _fitScale = 1.0f;      // 最終サイズ乗算

    // --- Linear Arrange ---
    Direction _arrangeDirection = Direction.X;
    float _arrangeSpacing = 1.0f;

    // --- Grid Arrange ---
    GridPlane _gridPlane = GridPlane.XZ;
    ArrangeOrderMode _arrangeOrder = ArrangeOrderMode.NameOrder;
    bool _centered = true;       // 最初のオブジェクトをグリッド中心にする
    bool _keepYHeightXZ = true;  // XZグリッド時に元のY高さを維持
    // Manual spacing
    float _spacingX = 1f;
    float _spacingY = 1f;
    float _spacingZ = 1f;
    // Auto spacing
    bool _autoSpacing = true;     // Renderer/Colliderから自動算出
    bool _uniformCell = true;     // 選択中の最大寸法で統一セル
    float _autoMargin = 0.05f;    // 追加余白（m）

    // --- Duplicate & Rewire ---
    string _sourcePath = "";
    string _targetPath = "";
    string _excludeCSV = "";

    // =========================
    // メニュー
    // =========================
    [MenuItem("IKA3DCG/IKA拡張設定")]
    static void Open() => GetWindow<IKA3DCGExpansionEditor>("IKA3DCG拡張ツール");

    // =========================
    // ライフサイクル
    // =========================
    void OnEnable() { Selection.selectionChanged += OnSelectionChanged; }
    void OnDisable() { Selection.selectionChanged -= OnSelectionChanged; }
    void OnSelectionChanged()
    {
        _selectionCount = Selection.objects.Length;
        _recentSelectionCache = Selection.gameObjects.ToList();
        Repaint();
    }

    // =========================
    // GUI
    // =========================
    void OnGUI()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        DrawTitle("コライダーの追加（生成）");
        DrawAddColliderSection();

        Space(16);
        DrawTitle("一括実行（生成／チェック）");
        DrawBulkButtons();

        Space(8);
        EditorGUILayout.LabelField($"選択オブジェクト数：{_selectionCount}");

        Space(16);
        DrawTitle("一直線に並べる");
        DrawLinearArrangeSection();

        Space(16);
        DrawTitle("グリッド配置");
        DrawGridArrangeSection();

        Space(16);
        DrawTitle("既存コライダーを子メッシュにフィット");
        DrawColliderFitSection();

        Space(16);
        DrawTitle("複製＋参照更新（GUID差し替え）");
        DrawDuplicateRewireSection();

        EditorGUILayout.EndScrollView();
    }

    // ---------- GUI セクション ----------
    void DrawTitle(string title)
    {
        GUILayout.Label($"=== {title} ===", EditorStyles.boldLabel);
    }

    void Space(int px) => GUILayout.Space(px);

    void DrawAddColliderSection()
    {
        _addStringPos = (AddStringPosition)EditorGUILayout.EnumPopup("追加する名前の位置", _addStringPos);
        if (_addStringPos != AddStringPosition.None)
            _addName = EditorGUILayout.TextField("追加する文字列", _addName);

        _colliderType = (ColliderType)EditorGUILayout.EnumPopup("生成コライダーの種類", _colliderType);
        if (_colliderType == ColliderType.Sphere)
            _sphereBasis = (SphereColliderBasis)EditorGUILayout.EnumPopup("球コライダーの半径基準", _sphereBasis);

        _colliderScale = EditorGUILayout.Slider("生成コライダーのサイズ倍率", _colliderScale, 0.5f, 5f);

        Space(6);
        _addVRCPickup = EditorGUILayout.ToggleLeft("VRChat Pickup を追加（SDKが無い場合は自動スキップ）", _addVRCPickup);
        if (_addVRCPickup)
        {
            _pickupAutoHold = EditorGUILayout.ToggleLeft("Pickup: AutoHold を Yes にする", _pickupAutoHold);
            _rigidUseGravity = EditorGUILayout.ToggleLeft("Rigidbody: Gravity 有効", _rigidUseGravity);
            _rigidIsKinematic = EditorGUILayout.ToggleLeft("Rigidbody: Kinematic 有効", _rigidIsKinematic);
            _addObjectSync = EditorGUILayout.ToggleLeft("VRC ObjectSync を追加", _addObjectSync);
        }
    }

    void DrawBulkButtons()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("実行（選択の直下に新規親＋コライダー生成）"))
                SetColliderSizeAdjustment();

            if (GUILayout.Button("実行（子階層の頂点を集約して生成）"))
                SetColliderSizeAdjustmentChild();
        }

        if (GUILayout.Button("子メッシュ寸法から、既存コライダーのサイズを設定"))
            CollSizeSettingFromChildMeshSize();
    }

    void DrawLinearArrangeSection()
    {
        _arrangeDirection = (Direction)EditorGUILayout.EnumPopup("方向", _arrangeDirection);
        _arrangeSpacing = EditorGUILayout.FloatField("等間隔（m）", _arrangeSpacing);
        if (GUILayout.Button("一直線に配置"))
            ArrangeSelectedObjects();
    }

    void DrawGridArrangeSection()
    {
        _gridPlane = (GridPlane)EditorGUILayout.EnumPopup("配置する平面", _gridPlane);
        _arrangeOrder = (ArrangeOrderMode)EditorGUILayout.EnumPopup("並べ順", _arrangeOrder);

        _autoSpacing = EditorGUILayout.ToggleLeft("自動間隔（Renderer/ColliderのワールドBoundsから算出）", _autoSpacing);
        if (_autoSpacing)
        {
            _autoMargin = EditorGUILayout.FloatField("自動間隔に加える余白（m）", _autoMargin);
            _uniformCell = EditorGUILayout.ToggleLeft("セルサイズを最大寸法で統一", _uniformCell);
        }
        else
        {
            _spacingX = EditorGUILayout.FloatField("間隔 X（m）", _spacingX);
            if (_gridPlane == GridPlane.XY) _spacingY = EditorGUILayout.FloatField("間隔 Y（m）", _spacingY);
            if (_gridPlane == GridPlane.XZ || _gridPlane == GridPlane.YZ)
                _spacingZ = EditorGUILayout.FloatField("間隔 Z（m）", _spacingZ);
        }

        _centered = EditorGUILayout.ToggleLeft("先頭オブジェクト位置をグリッド中心にする", _centered);
        if (_gridPlane == GridPlane.XZ)
            _keepYHeightXZ = EditorGUILayout.ToggleLeft("XZ 平面で元の高さ（Y）を維持", _keepYHeightXZ);

        if (GUILayout.Button("グリッド配置を実行"))
            ArrangeSelectedObjectsAsGrid();
    }

    void DrawColliderFitSection()
    {
        _fitPadding = EditorGUILayout.FloatField("パディング（ローカル各軸に±付与）", _fitPadding);
        _fitScale = EditorGUILayout.Slider("サイズ倍率（最終サイズに乗算）", _fitScale, 0.1f, 5f);

        if (GUILayout.Button("子の Renderer から【親の既存コライダー】をフィット"))
            FitExistingCollidersOnSelection();
    }

    void DrawDuplicateRewireSection()
    {
        _sourcePath = EditorGUILayout.TextField("複製元（絶対 or Assets/...）", _sourcePath);
        _targetPath = EditorGUILayout.TextField("複製先（絶対 or Assets/...）", _targetPath);
        _excludeCSV = EditorGUILayout.TextField("除外ディレクトリ（CSV／部分一致）", _excludeCSV);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("ディレクトリを複製＋参照更新"))
                DuplicateDirectoryWithRewire();
            if (GUILayout.Button("ファイルを複製＋参照更新"))
                DuplicateSingleFileWithRewire();
        }
    }

    // =========================
    // コライダー生成
    // =========================
    void SetColliderSizeAdjustment()
    {
        var objs = Selection.gameObjects;
        for (int i = 0; i < objs.Length; i++)
        {
            var childObj = objs[i];
            if (!TryUnpackPrefabDialog(childObj)) return;

            MeshFilter mf = childObj.GetComponent<MeshFilter>();
            if (mf && mf.sharedMesh) { CreateMesh(mf.sharedMesh, childObj); continue; }

            var smr = childObj.GetComponent<SkinnedMeshRenderer>();
            if (smr && smr.sharedMesh) { CreateMesh(smr.sharedMesh, childObj); continue; }

            Debug.Log($"{childObj.name} に Mesh が見つからなかったためスキップしました。");
        }
    }

    void SetColliderSizeAdjustmentChild()
    {
        var roots = Selection.gameObjects;
        int undo = 0;

        for (int i = 0; i < roots.Length; i++)
        {
            var childObj = roots[i];
            if (!TryUnpackPrefabDialog(childObj)) return;

            XYZ xyz = new XYZ();
            CheckForComponentRecursive(childObj, ref xyz);
            xyz.Calculation();

            GameObject parent = new GameObject();
            Undo.RegisterCreatedObjectUndo(parent, $"Create Parent{undo++}");

            parent.name = _addStringPos switch
            {
                AddStringPosition.First => $"{_addName}{childObj.name}",
                AddStringPosition.Last => $"{childObj.name}{_addName}",
                _ => childObj.name
            };

            Undo.SetTransformParent(parent.transform, childObj.transform.parent, $"Change Parent{undo++}");

            if (_colliderType == ColliderType.Box)
            {
                Vector3 core = new Vector3(((xyz.xMax - xyz.xMin) / 2) + xyz.xMin, ((xyz.yMax - xyz.yMin) / 2) + xyz.yMin, ((xyz.zMax - xyz.zMin) / 2) + xyz.zMin);
                Vector3 size = new Vector3(xyz.xMax - xyz.xMin, xyz.yMax - xyz.yMin, xyz.zMax - xyz.zMin);

                Undo.RecordObject(parent.transform, $"undo{undo++}");
                parent.transform.localPosition = childObj.transform.localPosition;

                Undo.SetTransformParent(childObj.transform, parent.transform, $"Change Parent{undo++}");
                childObj.transform.localPosition = Vector3.zero;

                BoxCollider box = Undo.AddComponent<BoxCollider>(parent);
                box.size = size * _colliderScale;
                box.center = core - childObj.transform.position;
            }
            else
            {
                float[] sizes = new float[] { xyz.xMax - xyz.xMin, xyz.yMax - xyz.yMin, xyz.zMax - xyz.zMin };
                float radius = _sphereBasis == SphereColliderBasis.Min ? sizes.Min() / 2f :
                               _sphereBasis == SphereColliderBasis.Average ? (sizes[0] + sizes[1] + sizes[2]) / 6f :
                               sizes.Max() / 2f;

                Vector3 core = new Vector3(((xyz.xMax - xyz.xMin) / 2) + xyz.xMin, ((xyz.yMax - xyz.yMin) / 2) + xyz.yMin, ((xyz.zMax - xyz.zMin) / 2) + xyz.zMin);

                Undo.RecordObject(parent.transform, $"undo{undo++}");
                parent.transform.position = core;

                Undo.SetTransformParent(childObj.transform, parent.transform, $"Change Parent{undo++}");

                SphereCollider sp = Undo.AddComponent<SphereCollider>(parent);
                sp.radius = radius * _colliderScale;
                sp.center = Vector3.zero;
            }

            if (_addVRCPickup) AddVRCPickupAndSync(parent);
        }
    }

    // ref なし版
    void CreateMesh(Mesh mesh, GameObject childObj)
    {
        List<Vector3> v = new List<Vector3>();
        mesh.GetVertices(v);
        if (v.Count <= 2)
        {
            Debug.Log($"{childObj.name} の頂点が少ないため、生成をスキップしました。");
            return;
        }

        XYZ xyz = new XYZ();
        for (int i = 0; i < v.Count; i++)
            xyz.Check(childObj.transform.TransformPoint(v[i]));

        int undo = 0;
        GameObject parent = new GameObject();
        Undo.RegisterCreatedObjectUndo(parent, $"Create Parent{undo++}");

        parent.name = _addStringPos switch
        {
            AddStringPosition.First => $"{_addName}{childObj.name}",
            AddStringPosition.Last => $"{childObj.name}{_addName}",
            _ => childObj.name
        };

        Undo.SetTransformParent(parent.transform, childObj.transform.parent, $"Change Parent{undo++}");

        if (_colliderType == ColliderType.Box)
        {
            Vector3 core = new Vector3(((xyz.xMax - xyz.xMin) / 2) + xyz.xMin, ((xyz.yMax - xyz.yMin) / 2) + xyz.yMin, ((xyz.zMax - xyz.zMin) / 2) + xyz.zMin);
            Vector3 size = new Vector3(xyz.xMax - xyz.xMin, xyz.yMax - xyz.yMin, xyz.zMax - xyz.zMin);

            Undo.RecordObject(parent.transform, $"undo{undo++}");
            parent.transform.localPosition = childObj.transform.localPosition;

            Undo.SetTransformParent(childObj.transform, parent.transform, $"Change Parent{undo++}");
            childObj.transform.localPosition = Vector3.zero;

            BoxCollider box = Undo.AddComponent<BoxCollider>(parent);
            box.size = size * _colliderScale;
            box.center = core - childObj.transform.position;
        }
        else
        {
            float[] sizes = new float[] { xyz.xMax - xyz.xMin, xyz.yMax - xyz.yMin, xyz.zMax - xyz.zMin };
            float radius = _sphereBasis == SphereColliderBasis.Min ? sizes.Min() / 2f :
                           _sphereBasis == SphereColliderBasis.Average ? (sizes[0] + sizes[1] + sizes[2]) / 6f :
                           sizes.Max() / 2f;

            Vector3 core = new Vector3(((xyz.xMax - xyz.xMin) / 2) + xyz.xMin, ((xyz.yMax - xyz.yMin) / 2) + xyz.yMin, ((xyz.zMax - xyz.zMin) / 2) + xyz.zMin);

            Undo.RecordObject(parent.transform, $"undo{undo++}");
            parent.transform.position = core;

            Undo.SetTransformParent(childObj.transform, parent.transform, $"Change Parent{undo++}");

            SphereCollider sp = Undo.AddComponent<SphereCollider>(parent);
            sp.radius = radius * _colliderScale;
            sp.center = Vector3.zero;
        }

        if (_addVRCPickup) AddVRCPickupAndSync(parent);
    }

    void CheckForComponentRecursive(GameObject obj, ref XYZ xyz)
    {
        MeshFilter mf = obj.GetComponent<MeshFilter>();
        if (mf && mf.sharedMesh)
        {
            List<Vector3> v = new List<Vector3>();
            mf.sharedMesh.GetVertices(v);
            for (int n = 0; n < v.Count; n++)
                xyz.Check(obj.transform.TransformPoint(v[n]));
        }
        foreach (Transform t in obj.transform)
            CheckForComponentRecursive(t.gameObject, ref xyz);
    }

    void CollSizeSettingFromChildMeshSize()
    {
        var objs = Selection.gameObjects;
        for (int i = 0; i < objs.Length; i++)
        {
            var go = objs[i];
            XYZ xyz = new XYZ();
            CheckForComponentRecursive(go, ref xyz);
            xyz.Calculation();

            if (_colliderType == ColliderType.Box)
            {
                Vector3 core = new Vector3(((xyz.xMax - xyz.xMin) / 2) + xyz.xMin, ((xyz.yMax - xyz.yMin) / 2) + xyz.yMin, ((xyz.zMax - xyz.zMin) / 2) + xyz.zMin);
                Vector3 size = new Vector3(xyz.xMax - xyz.xMin, xyz.yMax - xyz.yMin, xyz.zMax - xyz.zMin);

                BoxCollider box = go.GetComponent<BoxCollider>();
                if (!box) { Debug.LogWarning($"{go.name} に BoxCollider がありません"); continue; }

                Undo.RecordObject(box, "Fit BoxCollider");
                box.size = size * _colliderScale;
                box.center = core - go.transform.position;
            }
            else
            {
                float[] sizes = new float[] { xyz.xMax - xyz.xMin, xyz.yMax - xyz.yMin, xyz.zMax - xyz.zMin };
                float radius = _sphereBasis == SphereColliderBasis.Min ? sizes.Min() / 2f :
                               _sphereBasis == SphereColliderBasis.Average ? (sizes[0] + sizes[1] + sizes[2]) / 6f :
                               sizes.Max() / 2f;

                SphereCollider sp = go.GetComponent<SphereCollider>();
                if (!sp) { Debug.LogWarning($"{go.name} に SphereCollider がありません"); continue; }

                Undo.RecordObject(sp, "Fit SphereCollider");
                sp.radius = radius * _colliderScale;
                sp.center = Vector3.zero;
            }
        }
    }

    // =========================
    // 既存コライダーを子のRendererからフィット
    // =========================
    void FitExistingCollidersOnSelection()
    {
        var roots = Selection.gameObjects;
        if (roots == null || roots.Length == 0)
        {
            Debug.LogWarning("オブジェクトが選択されていません。");
            return;
        }

        int fitCount = 0;
        foreach (var root in roots)
        {
            Bounds localBounds;
            if (!TryComputeLocalBoundsFromChildrenRenderers(root, out localBounds))
            {
                Debug.LogWarning($"[{root.name}] 子階層に Renderer が見つからないためスキップ。");
                continue;
            }

            Vector3 sizeLocal = localBounds.size;
            sizeLocal += Vector3.one * (_fitPadding * 2f);
            sizeLocal *= Mathf.Max(0.0001f, _fitScale);
            Vector3 centerLocal = localBounds.center;

            bool adjusted = false;

            foreach (var box in root.GetComponents<BoxCollider>())
            {
                Undo.RecordObject(box, "Fit Existing BoxCollider");
                box.center = centerLocal;
                box.size = sizeLocal;
                adjusted = true;
            }

            var spheres = root.GetComponents<SphereCollider>();
            if (spheres != null && spheres.Length > 0)
            {
                float radius = 0.5f * Mathf.Max(sizeLocal.x, Mathf.Max(sizeLocal.y, sizeLocal.z));
                foreach (var sp in spheres)
                {
                    Undo.RecordObject(sp, "Fit Existing SphereCollider");
                    sp.center = centerLocal;
                    sp.radius = radius;
                    adjusted = true;
                }
            }

            if (!adjusted)
                Debug.LogWarning($"[{root.name}] 調整可能な Collider（Box/Sphere）が見つかりません。");
            else
                fitCount++;
        }

        Debug.Log($"既存Colliderフィット完了：{fitCount} / {roots.Length}");
    }

    // 子階層レンダラーのワールドAABB→親ローカルAABB
    bool TryComputeLocalBoundsFromChildrenRenderers(GameObject root, out Bounds localBounds)
    {
        localBounds = new Bounds(Vector3.zero, Vector3.zero);
        var renderers = root.GetComponentsInChildren<Renderer>(true);
        if (renderers == null || renderers.Length == 0) return false;

        bool hasAny = false;
        for (int i = 0; i < renderers.Length; i++)
        {
            Bounds wb = renderers[i].bounds; // ワールドAABB
            if (wb.size == Vector3.zero) continue;

            Vector3[] corners = GetBoundsWorldCorners(wb);
            for (int c = 0; c < corners.Length; c++)
            {
                Vector3 local = root.transform.InverseTransformPoint(corners[c]);
                if (!hasAny) { localBounds = new Bounds(local, Vector3.zero); hasAny = true; }
                else localBounds.Encapsulate(local);
            }
        }
        return hasAny;
    }

    Vector3[] GetBoundsWorldCorners(Bounds b)
    {
        Vector3 min = b.min;
        Vector3 max = b.max;
        return new Vector3[]
        {
            new Vector3(min.x, min.y, min.z),
            new Vector3(max.x, min.y, min.z),
            new Vector3(min.x, max.y, min.z),
            new Vector3(min.x, min.y, max.z),
            new Vector3(max.x, max.y, min.z),
            new Vector3(min.x, max.y, max.z),
            new Vector3(max.x, min.y, max.z),
            new Vector3(max.x, max.y, max.z),
        };
    }

    // =========================
    // 一方向整列
    // =========================
    void ArrangeSelectedObjects()
    {
        var selected = Selection.gameObjects;
        if (selected.Length == 0)
        {
            Debug.LogWarning("オブジェクトを選択してください。");
            return;
        }

        Array.Sort(selected, (a, b) => string.Compare(a.name, b.name, StringComparison.Ordinal));
        Vector3 pos = selected[0].transform.position;
        Vector3 step = GetDirectionVector();

        foreach (var go in selected)
        {
            Undo.RecordObject(go.transform, "Line Arrange");
            go.transform.position = pos;
            pos += step * _arrangeSpacing;
        }

        Debug.Log("一直線の配置を完了しました。");
    }

    Vector3 GetDirectionVector()
    {
        switch (_arrangeDirection)
        {
            case Direction.X: return Vector3.right;
            case Direction.Y: return Vector3.up;
            case Direction.Z: return Vector3.forward;
        }
        return Vector3.right;
    }

    // =========================
    // グリッド配置
    // =========================
    void ArrangeSelectedObjectsAsGrid()
    {
        var selected = Selection.gameObjects;
        if (selected == null || selected.Length == 0)
        {
            Debug.LogWarning("オブジェクトを選択してください。");
            return;
        }

        GameObject[] ordered = OrderSelection(selected, _arrangeOrder);
        int n = ordered.Length;

        int cols, rows;
        ComputeGrid(n, out cols, out rows);

        Vector3 origin = ordered[0].transform.position;

        Vector3 colAxis, rowAxis;
        GetGridSteps(out colAxis, out rowAxis);

        float sx = _spacingX, sy = _spacingY, sz = _spacingZ;
        if (_autoSpacing)
        {
            ComputeAutoSpacing(ordered, out sx, out sy, out sz);
            sx += _autoMargin; sy += _autoMargin; sz += _autoMargin;
        }

        Vector3 colDelta = ScaleAxis(colAxis, sx, sy, sz);
        Vector3 rowDelta = ScaleAxis(rowAxis, sx, sy, sz);

        Vector3 offset = _centered
            ? (-0.5f * (cols - 1)) * colDelta + (-0.5f * (rows - 1)) * rowDelta
            : Vector3.zero;

        for (int i = 0; i < n; i++)
        {
            int r = i / cols;
            int c = i % cols;
            Vector3 p = origin + offset + c * colDelta + r * rowDelta;

            if (_gridPlane == GridPlane.XZ && _keepYHeightXZ)
                p.y = ordered[i].transform.position.y;

            Undo.RecordObject(ordered[i].transform, "Grid Arrange");
            ordered[i].transform.position = p;
        }

        Debug.Log($"グリッド配置完了：{cols} x {rows} / {_gridPlane} / 自動間隔={_autoSpacing}");
    }

    GameObject[] OrderSelection(GameObject[] src, ArrangeOrderMode mode)
    {
        if (mode == ArrangeOrderMode.HierarchyOrder)
            return src.OrderBy(o => o.transform.GetHierarchyPath()).ToArray();
        if (mode == ArrangeOrderMode.SelectionOrder)
            return _recentSelectionCache.Where(x => src.Contains(x)).ToArray();
        return src.OrderBy(o => o.name, StringComparer.Ordinal).ToArray(); // NameOrder
    }

    void ComputeGrid(int n, out int cols, out int rows)
    {
        int sq = Mathf.CeilToInt(Mathf.Sqrt(n));
        int bestC = Mathf.Max(1, sq);
        int bestR = Mathf.CeilToInt((float)n / bestC);
        float best = AspectScore(bestC, bestR);

        for (int c = Mathf.Max(1, sq - 3); c <= sq + 3; c++)
        {
            int r = Mathf.CeilToInt((float)n / c);
            float s = AspectScore(c, r);
            if (s < best) { best = s; bestC = c; bestR = r; }
        }
        cols = bestC; rows = bestR;
    }

    float AspectScore(int cols, int rows)
    {
        if (cols == 0 || rows == 0) return float.MaxValue;
        float a = (float)cols / rows;
        return Mathf.Abs(a - 1f);
    }

    void GetGridSteps(out Vector3 col, out Vector3 row)
    {
        switch (_gridPlane)
        {
            case GridPlane.XY: col = Vector3.right; row = Vector3.up; break;
            case GridPlane.YZ: col = Vector3.forward; row = Vector3.up; break;
            default: col = Vector3.right; row = Vector3.forward; break; // XZ
        }
    }

    Vector3 ScaleAxis(Vector3 axis, float sx, float sy, float sz)
    {
        return new Vector3(
            axis.x != 0 ? Mathf.Sign(axis.x) * sx : 0f,
            axis.y != 0 ? Mathf.Sign(axis.y) * sy : 0f,
            axis.z != 0 ? Mathf.Sign(axis.z) * sz : 0f
        );
    }

    void ComputeAutoSpacing(GameObject[] items, out float sx, out float sy, out float sz)
    {
        float maxX = 0f, maxY = 0f, maxZ = 0f;
        for (int i = 0; i < items.Length; i++)
        {
            Bounds b;
            if (!TryGetWorldBounds(items[i], out b)) continue;

            Vector3 s = b.size;
            if (_gridPlane == GridPlane.XZ) { maxX = Mathf.Max(maxX, s.x); maxZ = Mathf.Max(maxZ, s.z); }
            else if (_gridPlane == GridPlane.XY) { maxX = Mathf.Max(maxX, s.x); maxY = Mathf.Max(maxY, s.y); }
            else { maxZ = Mathf.Max(maxZ, s.z); maxY = Mathf.Max(maxY, s.y); }
        }

        if (_uniformCell)
        {
            if (_gridPlane == GridPlane.XZ) { sx = maxX; sy = 0f; sz = maxZ; return; }
            if (_gridPlane == GridPlane.XY) { sx = maxX; sy = maxY; sz = 0f; return; }
            sx = 0f; sy = maxY; sz = maxZ; return; // YZ
        }

        if (_gridPlane == GridPlane.XZ) { sx = maxX; sy = 0f; sz = maxZ; return; }
        if (_gridPlane == GridPlane.XY) { sx = maxX; sy = maxY; sz = 0f; return; }
        sx = 0f; sy = maxY; sz = maxZ;           // YZ
    }

    bool TryGetWorldBounds(GameObject root, out Bounds worldBounds)
    {
        worldBounds = new Bounds(Vector3.zero, Vector3.zero);
        bool has = false;

        var rs = root.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < rs.Length; i++)
        {
            Bounds b = rs[i].bounds;
            if (!has) { worldBounds = b; has = true; } else worldBounds.Encapsulate(b);
        }

        if (!has)
        {
            var cs = root.GetComponentsInChildren<Collider>(true);
            for (int i = 0; i < cs.Length; i++)
            {
                Bounds b = cs[i].bounds;
                if (!has) { worldBounds = b; has = true; } else worldBounds.Encapsulate(b);
            }
        }
        return has;
    }

    // =========================
    // VRChat（リフレクションで安全追加）
    // =========================
    void AddVRCPickupAndSync(GameObject go)
    {
        Type pickupType = FindType("VRC.SDK3.Components.VRCPickup");
        Type objectSyncType = FindType("VRC.SDK3.Components.VRCObjectSync");

        if (pickupType == null && objectSyncType == null)
        {
            Debug.Log("[IKA] VRChat SDK が見つからないため、Pickup / ObjectSync の追加をスキップしました。");
            return;
        }

        Rigidbody rigi = go.GetComponent<Rigidbody>() ?? go.AddComponent<Rigidbody>();
        rigi.useGravity = _rigidUseGravity;
        rigi.isKinematic = _rigidIsKinematic;

        if (pickupType != null)
        {
            Component pickup = go.GetComponent(pickupType) ?? go.AddComponent(pickupType);

            if (_pickupAutoHold)
            {
                Type autoHoldEnum = FindType("VRC.SDKBase.VRC_Pickup+AutoHoldMode");
                if (autoHoldEnum != null)
                {
                    object yes = EnumParseSafe(autoHoldEnum, "Yes");
                    TrySetProperty(pickup, "AutoHold", yes);
                }
            }
        }

        if (_addObjectSync && objectSyncType != null && go.GetComponent(objectSyncType) == null)
            go.AddComponent(objectSyncType);
    }

    // ---- リフレクション補助 ----
    static Type FindType(string fullName)
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            try { var t = asm.GetType(fullName, false); if (t != null) return t; } catch { }
        }
        return null;
    }
    static object EnumParseSafe(Type t, string name)
    {
        try { return Enum.Parse(t, name, true); } catch { return Enum.GetValues(t).GetValue(0); }
    }
    static bool TrySetProperty(object obj, string prop, object val)
    {
        if (obj == null) return false;
        var p = obj.GetType().GetProperty(prop, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (p == null || !p.CanWrite) return false;
        try { p.SetValue(obj, val); return true; } catch { return false; }
    }

    // =========================
    // 複製＋参照更新
    // =========================
    async void DuplicateDirectoryWithRewire()
    {
        try
        {
            string origin = NormalizeAbsOrAssetsPath(_sourcePath);
            string target = NormalizeAbsOrAssetsPath(_targetPath);
            if (string.IsNullOrEmpty(origin) || string.IsNullOrEmpty(target))
            {
                Debug.LogError("複製元／複製先パスを指定してください。");
                return;
            }
            string[] exclude = ParseExcludeCsv(_excludeCSV);

            EditorUtility.DisplayProgressBar("コピー中", "アセットをコピーしています...", 0f);
            CopyDirectory(origin, target, exclude);
            AssetDatabase.Refresh();

            EditorUtility.DisplayProgressBar("参照更新", "GUIDを書き換えています...", 0.1f);
            await ChangeGuidToNewFile(origin, target, ShowEditorProgress);
            AssetDatabase.Refresh();

            Debug.Log($"ディレクトリ複製＋参照更新 完了\nFrom: {origin}\nTo  : {target}");
        }
        catch (Exception e) { Debug.LogException(e); }
        finally { EditorUtility.ClearProgressBar(); }
    }

    async void DuplicateSingleFileWithRewire()
    {
        try
        {
            string origin = NormalizeAbsOrAssetsPath(_sourcePath);
            string target = NormalizeAbsOrAssetsPath(_targetPath);
            if (string.IsNullOrEmpty(origin) || string.IsNullOrEmpty(target))
            {
                Debug.LogError("複製元／複製先パスを指定してください。");
                return;
            }

            CreateDirectoryIfNotExist(target);
            if (!AssetDatabase.CopyAsset(GetRelativePath(origin), GetRelativePath(target)))
            {
                Debug.LogError("ファイルのコピーに失敗しました。");
                return;
            }
            AssetDatabase.Refresh();

            string originDir = Path.GetDirectoryName(origin);
            string targetDir = Path.GetDirectoryName(target);

            EditorUtility.DisplayProgressBar("参照更新（単体）", "GUID対応表を作成...", 0.3f);
            var guidMap = CreateGuidMap(originDir, targetDir);

            string[] yamlExt = { ".anim", ".controller", ".overrideController", ".prefab", ".mat", ".material", ".playable", ".asset", ".unity" };
            if (yamlExt.Any(ext => target.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                await Task.Run(() =>
                {
                    var yaml = LoadYaml(target);
                    foreach (var doc in yaml.Documents)
                        ChangeGuidToNewFileRecursively(string.Empty, doc.RootNode, guidMap);

                    using (var sw = new StreamWriter(target, false, new UTF8Encoding(false)))
                        yaml.Save(sw, assignAnchors: false);

                    File.WriteAllText(target, ArrangeYaml(origin, target), new UTF8Encoding(false));
                });
            }

            AssetDatabase.Refresh();
            Debug.Log($"ファイル複製＋参照更新 完了\nFrom: {origin}\nTo  : {target}");
        }
        catch (Exception e) { Debug.LogException(e); }
        finally { EditorUtility.ClearProgressBar(); }
    }

    public static void CopyDirectory(string originDirectory, string targetDirectory, string[] excludeDirs = null)
    {
        DirectoryInfo dir = new DirectoryInfo(originDirectory);
        if (!dir.Exists) return;

        if (!Directory.Exists(targetDirectory))
            Directory.CreateDirectory(targetDirectory);

        FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);
        foreach (var fi in files)
        {
            if (fi.Extension == ".meta") continue;
            if (excludeDirs != null && excludeDirs.Any(x => fi.FullName.Contains(x))) continue;

            string dstDir = fi.DirectoryName.Replace(originDirectory, targetDirectory);
            string dst = Path.Combine(dstDir, fi.Name);
            CreateDirectoryIfNotExist(dst);
            AssetDatabase.CopyAsset(GetRelativePath(fi.FullName), GetRelativePath(dst));
        }
    }

    public static async Task ChangeGuidToNewFile(string originDirectory, string targetDirectory, Action<(int progress, int total)> cb = null)
    {
        var guidMap = CreateGuidMap(originDirectory, targetDirectory);
        var targetExt = new[] { ".anim", ".controller", ".overrideController", ".prefab", ".mat", ".material", ".playable", ".asset", ".unity" };

        var newPaths = Directory.GetFiles(targetDirectory, "*", SearchOption.AllDirectories)
                                .Where(p => !p.EndsWith(".meta"))
                                .ToList();

        var sync = SynchronizationContext.Current ?? new SynchronizationContext();
        object lockObj = new object();
        int total = newPaths.Count, progress = 0;

        List<Task> tasks = new List<Task>();
        foreach (var path in newPaths)
        {
            if (targetExt.All(ext => !path.EndsWith(ext, StringComparison.OrdinalIgnoreCase))) continue;

            tasks.Add(Task.Run(() =>
            {
                var yaml = LoadYaml(path);
                foreach (var doc in yaml.Documents)
                    ChangeGuidToNewFileRecursively(string.Empty, doc.RootNode, guidMap);

                using (var sw = new StreamWriter(path, false, new UTF8Encoding(false)))
                    yaml.Save(sw, assignAnchors: false);

                File.WriteAllText(path, ArrangeYaml(path.Replace(targetDirectory, originDirectory), path), new UTF8Encoding(false));

                sync.Post(_ =>
                {
                    lock (lockObj) { progress++; cb?.Invoke((progress, total)); }
                }, null);
            }));
        }
        await Task.WhenAll(tasks);
    }

    static Dictionary<string, string> CreateGuidMap(string originDirectory, string targetDirectory)
    {
        var map = new Dictionary<string, string>();
        var originPaths = Directory.GetFiles(originDirectory, "*", SearchOption.AllDirectories).Where(p => !p.EndsWith(".meta")).ToList();
        var newPaths = Directory.GetFiles(targetDirectory, "*", SearchOption.AllDirectories).Where(p => !p.EndsWith(".meta")).ToList();

        foreach (var newPath in newPaths)
        {
            string rel = newPath.Replace(targetDirectory, "");
            string originPath = originPaths.FirstOrDefault(p => p.Contains(rel));
            if (string.IsNullOrEmpty(originPath)) continue;

            string og = AssetDatabase.GUIDFromAssetPath(GetRelativePath(originPath)).ToString();
            string ng = AssetDatabase.GUIDFromAssetPath(GetRelativePath(newPath)).ToString();
            if (!string.IsNullOrEmpty(ng)) map[og] = ng;
        }
        return map;
    }

    static void ChangeGuidToNewFileRecursively(string nodeKey, YamlNode node, Dictionary<string, string> map)
    {
        switch (node.NodeType)
        {
            case YamlNodeType.Mapping:
                foreach (var entry in ((YamlMappingNode)node).Children)
                    ChangeGuidToNewFileRecursively(((YamlScalarNode)entry.Key).Value, entry.Value, map);
                break;

            case YamlNodeType.Sequence:
                foreach (var n in ((YamlSequenceNode)node).Children)
                    ChangeGuidToNewFileRecursively(string.Empty, n, map);
                break;

            case YamlNodeType.Scalar:
                if (nodeKey == "guid")
                {
                    var s = (YamlScalarNode)node;
                    if (!string.IsNullOrEmpty(s.Value) && map.TryGetValue(s.Value, out var newGuid))
                        s.Value = newGuid;
                }
                break;
        }
    }

    // =========================
    // 共通ユーティリティ
    // =========================
    static YamlStream LoadYaml(string path)
    {
        var input = new StringReader(File.ReadAllText(path));
        var yaml = new YamlStream();
        yaml.Load(input);
        return yaml;
    }

    static string ArrangeYaml(string originPath, string newPath)
    {
        StringBuilder sb = new StringBuilder();
        string header = @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
";
        sb.Append(header);

        using (var origin = new StringReader(File.ReadAllText(originPath)))
        using (var now = new StringReader(File.ReadAllText(newPath)))
        {
            Queue<string> divs = new Queue<string>();
            while (origin.Peek() > -1)
            {
                string line = origin.ReadLine();
                if (line.StartsWith("---")) divs.Enqueue(line);
            }

            if (divs.Count > 0) sb.AppendLine(divs.Dequeue());

            while (now.Peek() > -1)
            {
                string line = now.ReadLine();
                if (line.StartsWith("...")) continue;
                if (line.StartsWith("---"))
                {
                    if (divs.Count > 0) sb.AppendLine(divs.Dequeue());
                }
                else sb.AppendLine(line);
            }
        }
        return sb.ToString();
    }

    static string NormalizeAbsOrAssetsPath(string input)
    {
        if (string.IsNullOrEmpty(input)) return null;
        if (input.StartsWith("Assets/"))
            return Path.GetFullPath(Path.Combine(Directory.GetParent(Application.dataPath).FullName, input));
        if (Path.IsPathRooted(input)) return input;
        return null;
    }

    static string GetRelativePath(string path)
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        string ret = path.Replace(root, "");
        if (ret.StartsWith(Path.DirectorySeparatorChar.ToString())) ret = ret.Remove(0, 1);
        return ret;
    }

    static void CreateDirectoryIfNotExist(string filePath)
    {
        string dir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
    }

    static string[] ParseExcludeCsv(string csv)
    {
        if (string.IsNullOrEmpty(csv)) return null;
        return csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                  .Select(s => s.Trim())
                  .ToArray();
    }

    static void ShowEditorProgress((int progress, int total) p)
    {
        float prog = p.total > 0 ? (float)p.progress / p.total : 1f;
        EditorUtility.DisplayProgressBar("参照更新", $"{p.progress}/{p.total}", prog);
    }

    bool TryUnpackPrefabDialog(GameObject childObj)
    {
        if (PrefabUtility.GetOutermostPrefabInstanceRoot(childObj) == null) return true;

        int pressed = EditorUtility.DisplayDialogComplex(
            "Prefab 内のオブジェクトです",
            $"【{childObj.name}】は【{PrefabUtility.GetOutermostPrefabInstanceRoot(childObj).name}】のPrefab内にあります。Prefabを解除しますか？",
            "はい（解除して続行）", "キャンセル", "いいえ（解除せず続行）"
        );

        if (pressed == 0)
        {
            PrefabUtility.UnpackPrefabInstance(childObj.transform.parent.gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
            return true;
        }
        else if (pressed == 1) return false; // キャンセル→全体中断
        return true;                          // いいえ→スキップせず続行
    }
}

// =========================
// ヒエラルキー順比較ユーティリティ
// =========================
public static class TransformPathUtil
{
    public static string GetHierarchyPath(this Transform t)
    {
        StringBuilder sb = new StringBuilder();
        while (t != null)
        {
            sb.Insert(0, "/" + t.GetSiblingIndex().ToString("D6") + ":" + t.name);
            t = t.parent;
        }
        return sb.ToString();
    }
}
