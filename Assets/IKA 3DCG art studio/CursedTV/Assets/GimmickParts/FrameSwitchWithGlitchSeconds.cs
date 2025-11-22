using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class FrameSwitchWithGlitchSeconds : UdonSharpBehaviour
{
    [Header("Targets")]
    public Renderer _targetRenderer;     // ON時の画面（_UseManualFrame/_ManualFrame/_GlitchStartTime）
    public int _materialIndex = 0;       // _targetRenderer のマテリアルスロット
    public Animator _targetAnimator;     // ON時に動かすアニメータ（OFF時は Reset を打って停止）

    [Header("Switch Roots")]
    public GameObject _onObjectRoot;     // ON表示ルート
    public GameObject _offObjectRoot;    // OFF表示ルート

    [Header("Sheet Layout (for ON)")]
    public int _columns = 2;
    public int _rows = 2;

    [Header("Auto Advance (seconds, optional, ON only)")]
    public bool _autoAdvance = false;
    public float _intervalSeconds = 0.25f;

    [Header("Auto Triggers (2 slots, ON only)")]
    public int _fireOnFrame1 = -1;       // -1で無効
    public string _triggerName1 = "Shot";
    public int _fireOnFrame2 = -1;       // -1で無効
    public string _triggerName2 = "Finish";

    [Header("Audio per Frame (ON only)")]
    public AudioSource _audioSource;
    public int[] _soundOnFrames;
    public AudioClip[] _soundClips;

    [Header("Power / Animator")]
    public string _animResetTrigger = "Reset"; // OFF時に打つトリガー名

    [Header("State (local cache)")]
    public int _currentFrame = 0;

    // ======== 同期値（FieldChangeCallback で適用） ========
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(IsOn))] public bool _isOn = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(SyncedFrame))] public int _syncedFrame = 0;

    MaterialPropertyBlock _mpb;
    float _accum = 0f;

    // ======== プロパティ（ここで適用を一元管理） ========

    public bool IsOn
    {
        get => _isOn;
        set
        {
            bool changed = (_isOn != value);
            _isOn = value;

            // 電源OFF→ONで毎回 0 から再生（ご要望どおり）
            if (_isOn)
            {
                _accum = 0f;
                SyncedFrame = 0; // setter経由で反映（ONなのでグリッチ発火）
                _ApplyPowerStateLocal(true, true); // 先頭フレームにグリッチ＆音
            }
            else
            {
                _accum = 0f;
                // 即座に先頭へ戻す（ON復帰後も先頭から）
                _currentFrame = 0;
                // フレームは setter 経由だとグリッチが走るので、OFF時は直接ローカル保持だけでOK
                _syncedFrame = 0;

                // OFF見た目へ（グリッチ無し）
                _ApplyPowerStateLocal(false, false);
            }
        }
    }

    public int SyncedFrame
    {
        get => _syncedFrame;
        set
        {
            int clamped = Mathf.Clamp(value, 0, _Total() - 1);
            if (_syncedFrame == clamped) { return; }

            _syncedFrame = clamped;
            _currentFrame = _syncedFrame;

            // ON中のみ、フレーム切替の瞬間にグリッチ＆（必要であれば）効果音/トリガを走らせる
            if (_isOn)
            {
                _ApplyShader(_currentFrame, true);
            }
        }
    }

    // ======== ライフサイクル ========

    void Start()
    {
        if (_targetRenderer == null) _targetRenderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();

        // 起動時の見た目を同期値で反映（初期はOFFにしている）
        _currentFrame = _syncedFrame;
        _ApplyPowerStateLocal(_isOn, false);
    }

    void Update()
    {
        if (!_isOn) return;

        if (_autoAdvance && Networking.IsOwner(gameObject))
        {
            _accum += Time.deltaTime;
            if (_accum >= Mathf.Max(0.0001f, _intervalSeconds))
            {
                _accum = 0f;
                // ここはプロパティを通す：setterで描画＆効果が走る
                SyncedFrame = (_syncedFrame + 1) % _Total();
                RequestSerialization();
            }
        }
    }

    // 参加者に最新状態を配るだけ（FieldChangeCallback が適用処理を行う）
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (!Networking.IsOwner(gameObject)) return;
        RequestSerialization();
    }

    // ======== 外部Udonから呼ぶAPI（値セット→シリアライズのみ） ========

    public void _TogglePowerNet()
    {
        if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        IsOn = !IsOn;
        RequestSerialization();
    }

    public void _PowerOnNet()
    {
        if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (IsOn) return;
        IsOn = true;
        RequestSerialization();
    }

    public void _PowerOffNet()
    {
        if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (!IsOn) return;
        IsOn = false;
        RequestSerialization();
    }

    public void SetFrameNet(int frame)
    {
        if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SyncedFrame = frame;      // setter内で適用 & グリッチ（ON時）
        RequestSerialization();
    }

    public void NextFrameNet()
    {
        if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SyncedFrame = (_syncedFrame + 1) % _Total();
        RequestSerialization();
    }

    public void PrevFrameNet()
    {
        if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SyncedFrame = (_syncedFrame - 1 + _Total()) % _Total();
        RequestSerialization();
    }

    // ======== 内部処理 ========

    int _Total() { return Mathf.Max(1, _columns * _rows); }

    void _ApplyPowerStateLocal(bool isOn, bool allowGlitchOnApply)
    {
        if (_onObjectRoot != null) _onObjectRoot.SetActive(isOn);
        if (_offObjectRoot != null) _offObjectRoot.SetActive(!isOn);

        if (_targetAnimator != null)
        {
            if (isOn)
            {
                _targetAnimator.speed = 1f;
            }
            else
            {
                if (!string.IsNullOrEmpty(_animResetTrigger))
                    _targetAnimator.SetTrigger(_animResetTrigger);
                _targetAnimator.speed = 0f;
            }
        }

        if (!isOn && _audioSource != null)
            _audioSource.Stop();

        if (isOn)
            _ApplyShader(_currentFrame, allowGlitchOnApply);
    }

    void _ApplyShader(int frame, bool startGlitchNow)
    {
        if (_targetRenderer == null) return;

        if (_mpb == null) _mpb = new MaterialPropertyBlock();
        _mpb.Clear();
        _targetRenderer.GetPropertyBlock(_mpb, _materialIndex);

        _mpb.SetFloat("_UseManualFrame", 1f);
        _mpb.SetFloat("_ManualFrame", Mathf.Clamp(frame, 0, _Total() - 1));

        if (startGlitchNow)
            _mpb.SetFloat("_GlitchStartTime", Time.time);

        _targetRenderer.SetPropertyBlock(_mpb, _materialIndex);

        if (startGlitchNow && _targetAnimator != null)
        {
            if (frame == _fireOnFrame1 && !string.IsNullOrEmpty(_triggerName1))
                _targetAnimator.SetTrigger(_triggerName1);
            if (frame == _fireOnFrame2 && !string.IsNullOrEmpty(_triggerName2))
                _targetAnimator.SetTrigger(_triggerName2);
        }

        if (startGlitchNow && _audioSource != null && _soundOnFrames != null && _soundClips != null)
        {
            int len = _soundOnFrames.Length;
            int lenClips = _soundClips.Length;
            for (int i = 0; i < len; i++)
            {
                if (i >= lenClips) break;
                if (_soundOnFrames[i] != frame) continue;
                AudioClip clip = _soundClips[i];
                if (clip == null) continue;
                _audioSource.PlayOneShot(clip, 1f);
            }
        }
    }
}
