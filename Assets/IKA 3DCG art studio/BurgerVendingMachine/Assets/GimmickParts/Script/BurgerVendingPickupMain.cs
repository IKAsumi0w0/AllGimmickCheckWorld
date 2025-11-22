using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BurgerVendingPickupMain : UdonSharpBehaviour
{
    public BurgerVendingPickupSub _sub;
    [SerializeField] VRC_Pickup _pickup;
    [SerializeField] BoxCollider _coll;

    [Header("Mesh Objects (A=通常, B=切替後)")]
    [SerializeField] MeshRenderer _meshA;
    [SerializeField] MeshRenderer _meshB;

    [Header("Sound")]
    [Tooltip("MeshStateが1または2に切り替わった時に鳴るAudioSource")]
    [SerializeField] AudioSource _audioSource;

    // --- 状態管理: 0=A表示, 1=B表示, 2=リセット(非表示) ---
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(MeshState))]
    int _meshState = 2;

    // ====== プロパティ ======
    public int MeshState
    {
        get => _meshState;
        set
        {
            _meshState = value;
            ApplyMeshState();

            // --- 音再生トリガー（全員で同期再生される） ---
            if (MeshState == 1 || MeshState == 2)
            {
                PlaySyncSound();
            }
        }
    }

    // ====== メッシュ・コライダー表示更新 ======
    void ApplyMeshState()
    {
        if (_meshA == null || _meshB == null) return;

        // 0=A表示, 1=B表示, 2=非表示
        bool showA = MeshState == 0;
        bool showB = MeshState == 1;
        bool enableCollider = MeshState != 2;

        _meshA.enabled = showA;
        _meshB.enabled = showB;

        // コライダーは状態2（リセット時）のみ無効
        if (_coll != null)
            _coll.enabled = enableCollider;
    }

    // ====== ピックアップイベント ======
    public void MainPickup()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    public void MainDrop()
    {
        // 特に処理なし
    }

    public void MainPickupUseDown()
    {
        VRCPlayerApi lp = Networking.LocalPlayer;
        if (lp == null || !lp.IsOwner(gameObject)) return;

        // 状態を 0→1→2→0 の順でループ
        MeshState = (MeshState + 1) % 3;

        // 2=リセット状態 のときはリセット処理を実行
        if (MeshState == 2)
        {
            LocalReset();
        }

        RequestSerialization();
    }

    // ====== リセット処理 ======
    void LocalReset()
    {
        if (_pickup != null)
            _pickup.Drop();

        // Subを初期位置・回転に戻す
        if (_sub != null)
        {
            _sub.gameObject.transform.localPosition = Vector3.zero;
            _sub.gameObject.transform.localRotation = Quaternion.identity;
        }

        RequestSerialization();
    }

    // ====== 同期サウンド再生 ======
    void PlaySyncSound()
    {
        // 音声が設定されていれば再生
        if (_audioSource != null)
        {
            _audioSource.Stop(); // 前の音をリセット
            _audioSource.Play();
        }
    }

    // ====== プレイヤー参加時 ======
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        ApplyMeshState(); // 新規参加者にも反映
    }
}
