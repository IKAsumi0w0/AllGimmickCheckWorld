using UdonSharp;
using UnityEngine;
using UnityEngine.Animations;
using VRC.SDKBase;
using VRC.SDK3.Components;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PhotoCardController : UdonSharpBehaviour
{
    [Header("必須")]
    public VRCPickup _pickup;
    public Collider _collider;
    public ParentConstraint _constraint;

    [Header("写真オブジェクトの表示切替用（任意）")]
    public GameObject _photoRoot;

    [Header("Quadレンダラー")]
    public Renderer _frameRenderer;   // ← 1つに戻す
    public Renderer _photoRenderer;

    [Header("テクスチャプロパティ（URPは _BaseMap）")]
    public string _frameTexturePropertyName = "_MainTex";
    public string _photoTexturePropertyName = "_MainTex";

    [Header("フレームアトラス（A/Bの2枚を想定。各2x2=4コマ）")]
    public Texture _frameAtlasA;   // フレーム 0-3 用
    public int _atlasColumnsA = 2;
    public int _atlasRowsA = 2;

    public Texture _frameAtlasB;   // フレーム 4-7 用
    public int _atlasColumnsB = 2;
    public int _atlasRowsB = 2;

    [Header("フレームごとの写真Quadローカル変換（静的：同期不要）")]
    public Vector3[] _photoLocalPosByFrame;    // 0..7
    public Vector3[] _photoLocalScaleByFrame;  // 0..7

    [Header("排出パラメータ")]
    public float _ejectDistance = 0.25f;
    public float _ejectDuration = 0.25f;

    // 表示/当たり/Constraint の状態を同期（参照そのものは同期不可）
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(_synShown))] bool _shown;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(_synPickup))] bool _pickupable;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(_synConstr))] bool _constrActive = true;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(_syncedFrameIndex))]
    public int _frameIndex; // 0..7

    public int _frameCount = 8; // 2x2(A) + 2x2(B) を想定

    // 内部
    float _ejectTimer;
    bool _isEjecting;
    Vector3 _ejectBaseOffset;   // 0 基準
    Vector3 _ejectAxisLocal;    // +Z
    Transform _ejectSource;

    // ---------- 初期 ----------
    void Start()
    {
        ApplyShown(false);
        ApplyPickupable(false);
        ApplyConstraintActive(true);

        if (_frameCount < 1) _frameCount = 8;

        ApplyFrameVisual(_frameIndex);
        ApplyPhotoTransformForFrame(_frameIndex);
    }

    // ---------- 同期FC ----------
    public bool _synShown { get { return _shown; } set { _shown = value; ApplyShown(_shown); } }
    public bool _synPickup { get { return _pickupable; } set { _pickupable = value; ApplyPickupable(_pickupable); } }
    public bool _synConstr { get { return _constrActive; } set { _constrActive = value; ApplyConstraintActive(_constrActive); } }

    // ---------- 表示/当たり ----------
    void ApplyShown(bool on)
    {
        if (_photoRoot != null && _photoRoot != gameObject)
        {
            _photoRoot.SetActive(on);
        }
        else
        {
            if (_frameRenderer) _frameRenderer.enabled = on;
            if (_photoRenderer) _photoRenderer.enabled = on;
        }
    }

    void ApplyPickupable(bool on)
    {
        if (_collider) _collider.enabled = on;
        if (_pickup) _pickup.pickupable = on;
    }

    void ApplyConstraintActive(bool on)
    {
        if (_constraint) _constraint.constraintActive = on;
    }

    public void HideCard()
    {
        _shown = false; _pickupable = false;
        RequestSerialization();
        ApplyShown(false);
        ApplyPickupable(false);
    }
    public void ShowCard()
    {
        _shown = true; _pickupable = true;
        RequestSerialization();
        ApplyShown(true);
        ApplyPickupable(true);
    }

    // 全員でConstraint OFF/ONしたい時はネットワークイベントで呼ぶ
    public void NetConstraintOff() { _constrActive = false; ApplyConstraintActive(false); }
    public void NetConstraintOn() { _constrActive = true; ApplyConstraintActive(true); }

    public void ResetForReuse(Transform ejectOrigin)
    {
        if (_pickup != null && _pickup.IsHeld) _pickup.Drop();
        HideCard();

        if (_constraint != null && ejectOrigin != null)
        {
            _ejectSource = ejectOrigin;

            ConstraintSource src = new ConstraintSource();
            src.sourceTransform = _ejectSource;
            src.weight = 1f;

            if (_constraint.sourceCount == 0) _constraint.AddSource(src);
            else _constraint.SetSource(0, src);

            _constraint.SetTranslationOffset(0, Vector3.zero);
            _constraint.constraintActive = true;
            _constrActive = true;
            RequestSerialization();
        }

        _isEjecting = false;
        _ejectTimer = 0f;
    }

    // 排出（常に ejectOrigin の +Z / 基準0）
    public void BeginEject(Transform ejectOrigin, bool considerPrefabRotation)
    {
        if (_constraint == null || ejectOrigin == null) return;

        _ejectSource = ejectOrigin;

        ConstraintSource src = new ConstraintSource();
        src.sourceTransform = _ejectSource;
        src.weight = 1f;

        if (_constraint.sourceCount == 0) _constraint.AddSource(src);
        else _constraint.SetSource(0, src);

        _ejectBaseOffset = Vector3.zero;
        _ejectAxisLocal = Vector3.forward;

        _constraint.SetTranslationOffset(0, _ejectBaseOffset);
        _constraint.constraintActive = true;
        _constrActive = true;
        RequestSerialization();

        _ejectTimer = 0f;
        _isEjecting = true;
    }

    void Update()
    {
        if (!_isEjecting || _constraint == null) return;

        _ejectTimer += Time.deltaTime;
        float t = (_ejectDuration > 0f) ? Mathf.Clamp01(_ejectTimer / _ejectDuration) : 1f;

        Vector3 offset = _ejectBaseOffset + _ejectAxisLocal * (_ejectDistance * t);
        _constraint.SetTranslationOffset(0, offset);

        if (t >= 1f) _isEjecting = false;
    }

    // ---------- 写真テクスチャ ----------
    public void SetPhotoTexture(Texture tex)
    {
        if (_photoRenderer == null) return;
        var mat = _photoRenderer.material;
        if (mat != null && !string.IsNullOrEmpty(_photoTexturePropertyName))
            mat.SetTexture(_photoTexturePropertyName, tex);
    }

    // ---------- フレーム切替（A/Bのテクスチャ差し替え＋UV） ----------
    public void ApplyFrameAndSync(int index)
    {
        _frameIndex = Wrap(index, _frameCount);
        RequestSerialization();
        ApplyFrameVisual(_frameIndex);
        ApplyPhotoTransformForFrame(_frameIndex);
    }

    public int _syncedFrameIndex
    {
        get { return _frameIndex; }
        set
        {
            _frameIndex = Wrap(value, _frameCount);
            ApplyFrameVisual(_frameIndex);
            ApplyPhotoTransformForFrame(_frameIndex);
        }
    }

    void ApplyFrameVisual(int index)
    {
        if (_frameRenderer == null) return;
        var mat = _frameRenderer.material;
        if (mat == null || string.IsNullOrEmpty(_frameTexturePropertyName)) return;

        // 0..3 → A、4..7 → B
        bool useA = (Wrap(index, _frameCount) < 4);
        int localIdx = useA ? index : (index - 4);

        // atlas & layout
        Texture atlas = useA ? _frameAtlasA : _frameAtlasB;
        int cols = useA ? Mathf.Max(1, _atlasColumnsA) : Mathf.Max(1, _atlasColumnsB);
        int rows = useA ? Mathf.Max(1, _atlasRowsA) : Mathf.Max(1, _atlasRowsB);

        // テクスチャ差し替え
        if (atlas != null) mat.SetTexture(_frameTexturePropertyName, atlas);

        // 2DアトラスのUV
        float sx = 1f / cols;
        float sy = 1f / rows;
        int col = localIdx % cols;
        int row = localIdx / cols;

        mat.SetTextureScale(_frameTexturePropertyName, new Vector2(sx, sy));
        mat.SetTextureOffset(_frameTexturePropertyName, new Vector2(col * sx, 1f - sy - row * sy));
    }

    void ApplyPhotoTransformForFrame(int index)
    {
        if (_photoRenderer == null) return;
        Transform t = _photoRenderer.transform;
        int i = Wrap(index, _frameCount);

        if (_photoLocalPosByFrame != null && i < _photoLocalPosByFrame.Length)
            t.localPosition = _photoLocalPosByFrame[i];

        if (_photoLocalScaleByFrame != null && i < _photoLocalScaleByFrame.Length)
            t.localScale = _photoLocalScaleByFrame[i];
    }

    // ---------- 補助 ----------
    int Wrap(int v, int mod)
    {
        if (mod <= 0) return 0;
        int r = v % mod;
        if (r < 0) r += mod;
        return r;
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            ApplyShown(_shown);
            ApplyPickupable(_pickupable);
            ApplyConstraintActive(_constrActive);

            ApplyFrameVisual(_frameIndex);
            ApplyPhotoTransformForFrame(_frameIndex);
            RequestSerialization();
        }
    }
}
