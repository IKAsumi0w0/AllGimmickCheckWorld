using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Serialization.OdinSerializer;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BurgerVendingMachineMain : UdonSharpBehaviour
{
#if UNITY_EDITOR
    // ── Editor-only: 自動生成UIの保持項目（シーン/Prefabに保存される）
    [SerializeField, HideInInspector] public BurgerVendingPickupSub[] _genPrefabs = new BurgerVendingPickupSub[6]; // 種類6固定
    [SerializeField, HideInInspector] public Transform _genParent = null;
    [SerializeField, HideInInspector] public int[] _genCounts = new int[6] { 10, 10, 10, 10, 10, 10 }; // 種類ごとの個数
    [SerializeField, HideInInspector] public string _genBaseName = "BurgerSub";
#endif

    #region スポーン関係（6種類固定）

    // 種類固定: 0..5
    public BurgerVendingPickupSub[] pools0;
    public BurgerVendingPickupSub[] pools1;
    public BurgerVendingPickupSub[] pools2;
    public BurgerVendingPickupSub[] pools3;
    public BurgerVendingPickupSub[] pools4;
    public BurgerVendingPickupSub[] pools5;

    // ユーティリティ：種類インデックスから配列を取得
    BurgerVendingPickupSub[] GetPoolByType(int type)
    {
        switch (type)
        {
            case 0: return pools0;
            case 1: return pools1;
            case 2: return pools2;
            case 3: return pools3;
            case 4: return pools4;
            case 5: return pools5;
            default: return null;
        }
    }

    // その種類に空き（MeshState==2）があるか？
    bool HasAvailableSub(int typeIndex)
    {
        BurgerVendingPickupSub[] list = GetPoolByType(typeIndex);
        if (list == null || list.Length == 0) return false;

        for (int i = 0; i < list.Length; i++)
        {
            BurgerVendingPickupSub sub = list[i];
            if (sub == null || sub._main == null) continue;
            // 直参照はOK（同アセンブリ前提）。もし U# の未実装に当たる場合は sub._main.IsAvailable() ラッパーに切替。
            if (sub._main.MeshState == 2) return true;
        }
        return false;
    }

    // start から巡回して、スポーン可能な種類インデックスを探す
    int FindNextPlayableTypeIndex(int start)
    {
        int len = (burgerTriggers != null) ? Mathf.Min(6, burgerTriggers.Length) : 0;
        if (len <= 0) return -1;

        for (int step = 0; step < len; step++)
        {
            int idx = (start + step) % len;
            if (HasAvailableSub(idx)) return idx;
        }
        return -1; // 全種類に空きなし
    }

    // NextIndex 種に属する空きの Sub を 1 つ起こす
    public void SpawnObj()
    {
        VRCPlayerApi lp = Networking.LocalPlayer;
        if (lp == null || !lp.IsOwner(gameObject)) return;

        BurgerVendingPickupSub[] list = GetPoolByType(NextIndex);
        if (list == null || list.Length == 0) return;

        for (int subIndex = 0; subIndex < list.Length; subIndex++)
        {
            BurgerVendingPickupSub sub = list[subIndex];
            if (sub == null || sub._main == null) continue;

            if (sub._main.MeshState == 2)
            {
                if (!Networking.IsOwner(lp, sub.gameObject)) Networking.SetOwner(lp, sub.gameObject);
                if (!Networking.IsOwner(lp, sub._main.gameObject)) Networking.SetOwner(lp, sub._main.gameObject);

                sub.transform.localPosition = Vector3.zero;
                sub.transform.localRotation = Quaternion.identity;

                sub._main.MeshState = 0;     // A表示へ
                sub._main.RequestSerialization();

                return;
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("debug")]
    public void DebugMethod()
    {
        Debug.Log("===== Pools Debug =====");
        for (int t = 0; t < 6; t++)
        {
            var list = GetPoolByType(t);
            int len = list == null ? 0 : list.Length;
            Debug.Log($"Type {t}: count={len}");
            if (list != null)
            {
                for (int i = 0; i < list.Length; i++)
                {
                    var sub = list[i];
                    Debug.Log($"  [{i}] {(sub ? sub.name : "null")}");
                }
            }
        }
    }
#endif

    #endregion

    #region アニメーション関係

    [Header("Particle Slots (Slot 0–23)")]
    public ParticleSystem[] particles = new ParticleSystem[24];

    [Header("Audio (for Particle + Audio)")]
    [Tooltip("パーティクル再生時に鳴らすAudioSource")]
    public AudioSource audioSource;
    [Tooltip("各スロットに対応するAudioClip")]
    public AudioClip[] audioClips = new AudioClip[24];

    [Header("Sound Only Audio")]
    [Tooltip("SoundXX() で再生する専用AudioSource")]
    public AudioSource soundAudioSource;
    [Tooltip("SoundXX() で再生するAudioClip群（インデックス対応）")]
    public AudioClip[] soundClips = new AudioClip[24];

    [Header("Background Music (Single Clip)")]
    [Tooltip("BGM を再生する専用 AudioSource（Inspector で Clip を1つだけ設定）")]
    public AudioSource bgmAudioSource;

    [Header("Burger Animation")]
    public Animator BurgerAnime;

    [Tooltip("本編アニメのTrigger名リスト（6種類以上あっても使用は先頭6つまで）")]
    public string[] burgerTriggers = new string[]
    {
        "SteakBurger",
        "ShrimpBurger",
        "SquidRingBurger",
        "ChickenBurger",
        "DoubleBurger",
        "CheeseBurger",
    };

    [Tooltip("幕が開くアニメーションのTrigger名")]
    public string curtainRisesName = "CurtainRises";

    // パーティクル＋SE
    public void Steam0() { PlayByIndex(0); }
    public void LettuceWaterSpray1() { PlayByIndex(1); }
    public void SteakOilBake2() { PlayByIndex(2); }
    public void Cheese3() { PlayByIndex(3); }
    public void LettuceWaterSpray4() { PlayByIndex(4); }
    public void Steam5() { PlayByIndex(5); }
    public void GrilledPineappleOilBake6() { PlayByIndex(6); }
    public void PattyOilBake7() { PlayByIndex(7); }
    public void TheaterChicken8() { PlayByIndex(8); }
    public void PattyUpOilBake9() { PlayByIndex(9); }
    public void CheeseUp10() { PlayByIndex(10); }
    public void DischargePortSteam11() { PlayByIndex(11); }

    // SEのみ
    public void Kyupi0() { PlaySoundByIndex(0); }
    public void Peta1() { PlaySoundByIndex(1); }
    public void Nyuxtu2() { PlaySoundByIndex(2); }
    public void RollClosure3() { PlaySoundByIndex(3); }
    public void ShowtimeBuzzer4() { PlaySoundByIndex(4); }

    [SerializeField] Collider _interactCollider;
    [SerializeField] string debugName = "SteakBurger";

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(IsPlaying))]
    bool _isPlaying = false;
    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            _isPlaying = value;
            if (_interactCollider != null) _interactCollider.enabled = !_isPlaying;
        }
    }

    void PlayByIndex(int i)
    {
        if (i < 0) return;

        if (i < particles.Length)
        {
            var ps = particles[i];
            if (ps != null)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Play(true);
            }
        }

        if (audioSource != null && i < audioClips.Length)
        {
            var clip = audioClips[i];
            if (clip != null)
            {
                audioSource.Stop();
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }

    void PlaySoundByIndex(int i)
    {
        if (soundAudioSource == null) return;
        if (i < 0 || i >= soundClips.Length) return;

        var clip = soundClips[i];
        if (clip == null) return;

        soundAudioSource.Stop();
        soundAudioSource.clip = clip;
        soundAudioSource.Play();
    }

    public void PlayBGM(bool restart = false)
    {
        if (bgmAudioSource == null) return;
        if (restart) bgmAudioSource.time = 0f;
        if (!bgmAudioSource.isPlaying) bgmAudioSource.Play();
    }
    public void BGM_Play() { PlayBGM(false); }
    public void BGM_PlayRestart() { PlayBGM(true); }

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(SyncedBurgerIndex))]
    int _syncedBurgerIndex = -1;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(NextIndex))]
    int _nextIndex = 0;

    public int SyncedBurgerIndex
    {
        get => _syncedBurgerIndex;
        set
        {
            _syncedBurgerIndex = value;
            OnSyncedBurgerIndexChanged();
        }
    }

    public int NextIndex
    {
        get => _nextIndex;
        set
        {
            _nextIndex = value;
        }
    }

    void OnSyncedBurgerIndexChanged()
    {
        if (_syncedBurgerIndex < 0 || _syncedBurgerIndex >= Mathf.Min(6, burgerTriggers.Length)) return;
        PlayBurgerAnimation(_syncedBurgerIndex);
    }

    void PlayBurgerAnimation(int index)
    {
        if (BurgerAnime == null) return;

        if (!string.IsNullOrEmpty(curtainRisesName))
            BurgerAnime.SetTrigger(curtainRisesName);

        BurgerAnime.ResetTrigger(burgerTriggers[index]);
        BurgerAnime.SetTrigger(burgerTriggers[index]);
    }

    public void InteractMain()
    {
        if (BurgerAnime == null || burgerTriggers == null || burgerTriggers.Length == 0) return;

        VRCPlayerApi lp = Networking.LocalPlayer;
        if (lp != null && !Networking.IsOwner(lp, gameObject))
            Networking.SetOwner(lp, gameObject);

        if (IsPlaying) return; // 再生中は無視

        IsPlaying = true;
        RequestSerialization();

        int typeCount = Mathf.Min(6, burgerTriggers.Length);

        int start = (NextIndex + 1) % typeCount;
        int playable = FindNextPlayableTypeIndex(start);
        if (playable == -1)
        {
            IsPlaying = false;
            RequestSerialization();
            return;
        }

        NextIndex = playable;
        SyncedBurgerIndex = playable;

        RequestSerialization();
    }

    public void OnBurgerAnimationEnd()
    {
        IsPlaying = false;
        RequestSerialization();
    }

    // BGM フェード
    float _fadeTime, _fadeDur, _fadeFrom;
    bool _fadeActive;
    bool _fadeStopOnEnd = true;
    float _restoreVolume = 1f;

    public void BGM_FadeOut() => BGM_FadeOutSub(1f);
    public void BGM_FadeOutSub(float durationSeconds)
    {
        if (bgmAudioSource == null) return;
        if (durationSeconds <= 0f)
        {
            bgmAudioSource.Stop();
            return;
        }

        _fadeDur = durationSeconds;
        _fadeTime = 0f;
        _fadeFrom = bgmAudioSource.volume <= 0f ? 1f : bgmAudioSource.volume;
        _restoreVolume = _fadeFrom;
        _fadeStopOnEnd = true;
        _fadeActive = true;
    }

    void Update()
    {
        if (!_fadeActive || bgmAudioSource == null) return;

        _fadeTime += Time.deltaTime;
        float t = Mathf.Clamp01(_fadeTime / _fadeDur);
        bgmAudioSource.volume = Mathf.Lerp(_fadeFrom, 0f, t);

        if (t >= 1f)
        {
            if (_fadeStopOnEnd) bgmAudioSource.Stop();
            bgmAudioSource.volume = _restoreVolume;
            _fadeActive = false;
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer == player)
        {
            if (_interactCollider != null)
                _interactCollider.enabled = !IsPlaying;
        }
    }

    #endregion
}
