using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces; // NetworkEventTarget

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Halloween_GhostCamera_Main : UdonSharpBehaviour
{
    [Header("入力")]
    public Camera _sourceCamera;

    [Header("ライブモニター")]
    public RawImage _cameraBodyUI;

    [Header("カード管理（エディタで生成→自動セット）")]
    public Transform _anchor;
    public PhotoCardController[] _cards;

    [Header("カードPrefab（エディタ拡張で使用）")]
    public GameObject _photoPrefab;

    [Header("排出開始位置（このTransformの位置からZ+へ）")]
    public Transform _ejectOrigin;
    public float _ejectDistance = 0.25f;
    public float _ejectDuration = 0.25f;

    [Header("解像度・AA")]
    public int _width = 1024;
    public int _height = 1024;
    public int _antiAliasing = 2;

    [Header("シャッター音（全員に同期）")]
    public AudioSource _shutterSource;

    // 内部
    RenderTexture _liveRT;
    RenderTexture[] _photoRTs;
    int _photoCount;
    int _nextIndex;
    int _frameCursor;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(_synNextIndexFC))]
    private int _synNextIndex;

    public int _synNextIndexFC
    {
        get { return _synNextIndex; }
        set
        {
            _synNextIndex = value;
            // 受信側でもローカルに即反映（範囲ケア）
            if (_photoCount > 0)
            {
                if (_synNextIndex < 0) _synNextIndex = 0;
                if (_synNextIndex >= _photoCount) _synNextIndex = _photoCount - 1;
                _nextIndex = _synNextIndex;
            }
            else
            {
                // _photoCount 未初期化の場合でも保持だけしておく
                _nextIndex = _synNextIndex;
            }
        }
    }

    void Start()
    {
        _liveRT = CreateRT(_width, _height, _antiAliasing);
        if (_sourceCamera) _sourceCamera.targetTexture = _liveRT;
        if (_cameraBodyUI) _cameraBodyUI.texture = _liveRT;

        _photoCount = (_cards != null) ? _cards.Length : 0;
        _photoRTs = new RenderTexture[_photoCount];

        for (int i = 0; i < _photoCount; i++)
            if (_cards[i]) _cards[i].HideCard();

        _nextIndex = 0;
        _frameCursor = 0;

        // オーナーが現在の nextIndex を配信しておく（Late Joiner 初期化用）
        if (Networking.IsOwner(gameObject))
        {
            _synNextIndex = _nextIndex;
            RequestSerialization();
        }
    }

    RenderTexture CreateRT(int w, int h, int aa)
    {
        var rt = new RenderTexture(w, h, 0);
        rt.antiAliasing = Mathf.Max(1, aa);
        rt.name = "GhostCam_RT";
        return rt;
    }

    // プレイヤーがシャッターを押した時に呼ばれる
    public void Shoot()
    {
        // 全員で同じ処理を走らせる
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(NetShoot));
    }

    // 全員のクライアントで呼ばれる
    public void NetShoot()
    {
        if (_photoCount == 0) return;

        int idx = _nextIndex;
        var card = _cards[idx];
        if (card != null)
        {
            // フレーム同期
            card.ApplyFrameAndSync(_frameCursor);

            // RenderTexture を保存（各クライアントのローカル RT）
            _photoRTs[idx] = _liveRT;
            card.SetPhotoTexture(_photoRTs[idx]);
            card.ShowCard();

            // 排出
            card._ejectDistance = _ejectDistance;
            card._ejectDuration = _ejectDuration;
            if (_ejectOrigin != null)
                card.BeginEject(_ejectOrigin, false);
        }

        // シャッター音を全員で再生
        if (_shutterSource != null && _shutterSource.clip != null)
            _shutterSource.PlayOneShot(_shutterSource.clip);

        // 次スロット & 次フレーム
        _nextIndex = (_nextIndex + 1) % _photoCount;
        int fc = (_cards.Length > 0 && _cards[0] != null) ? Mathf.Max(1, _cards[0]._frameCount) : 4;
        _frameCursor = (_frameCursor + 1) % fc;

        // ★ オーナーが _nextIndex を同期値として配信
        if (Networking.IsOwner(gameObject))
        {
            _synNextIndex = _nextIndex;
            RequestSerialization();
        }

        // 次に上書きされるカードを事前リセット
        var overwriteCard = _cards[_nextIndex];
        if (overwriteCard != null && _ejectOrigin != null)
            overwriteCard.ResetForReuse(_ejectOrigin);

        // ライブRT差し替え
        var nextRT = _photoRTs[_nextIndex];
        if (nextRT == null) nextRT = CreateRT(_width, _height, _antiAliasing);
        _liveRT = nextRT;

        if (_sourceCamera) _sourceCamera.targetTexture = _liveRT;
        if (_cameraBodyUI && _cameraBodyUI.texture != _liveRT) _cameraBodyUI.texture = _liveRT;
    }

    public override void OnPickupUseDown() => Shoot();

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.IsOwner(gameObject))
        {
            _synNextIndex = _nextIndex;
            RequestSerialization();
        }
    }
}
