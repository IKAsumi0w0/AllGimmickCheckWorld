
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ChopsticksOpen_Pickup : UdonSharpBehaviour
{
    [SerializeField] ChopsticksOpen_PickupSub _sub;
    public BoxCollider _collSub;
    public BoxCollider _collMain;
    public MeshRenderer _mr;
    [SerializeField] MeshRenderer _normalMR;
    [SerializeField] MeshRenderer _greenOnionMR;
    [SerializeField] MeshRenderer _mentaikoMR;
    [SerializeField] AudioSource _as;
    bool _pickupUseFlg = false;
    float _timer = 0f;
    float _resetDelay = 1f;
    int _resetCount = 0;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(MainCollState))] bool _mainCollState = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(SubCollState))] bool _subCollState = false;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(NormalFlg))] bool _normalFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(GreenOnionFlg))] bool _greenOnionFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(MentaikoFlg))] bool _mentaikoFlg = false;

    public bool MainCollState
    {
        get => _mainCollState;
        set
        {
            _mainCollState = value;
            _mr.enabled = value;
            _collMain.enabled = value;
        }
    }

    public bool SubCollState
    {
        get => _subCollState;
        set
        {
            _subCollState = value;
            _collSub.enabled = value;
        }
    }

    public bool NormalFlg
    {
        get => _normalFlg;
        set
        {
            _normalFlg = value;
            _normalMR.enabled = _normalFlg;
        }
    }

    public bool GreenOnionFlg
    {
        get => _greenOnionFlg;
        set
        {
            _greenOnionFlg = value;
            _greenOnionMR.enabled = _greenOnionFlg;
        }
    }

    public bool MentaikoFlg
    {
        get => _mentaikoFlg;
        set
        {
            _mentaikoFlg = value;
            _mentaikoMR.enabled = _mentaikoFlg;
        }
    }

    public int ResetCount
    {
        get => _resetCount;
        set
        {
            _resetCount = value;
            if (3 <= _resetCount)
            {
                Reset();
                _resetCount = 0;
            }
        }
    }

    void Start()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            MainCollState = false;
            SubCollState = false;
            NormalFlg = false;
            GreenOnionFlg = false;
            MentaikoFlg = false;
            RequestSerialization();
        }
    }

    void Update()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            if (0 < ResetCount)
            {
                _timer += Time.deltaTime;
                if (_timer >= _resetDelay)
                {
                    ResetCount = 0;
                    _timer = 0f;
                }
            }
        }
    }

    public void MainPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (!MainCollState)
        {
            MainCollState = true;
            RequestSerialization();
        }
        _pickupUseFlg = false;
    }

    public void MainPickupUseDown()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject) && !_pickupUseFlg)
        {
            if (NormalFlg)
            {
                NormalFlg = false;
                FuncPlaySE();
            }
            if (GreenOnionFlg)
            {
                GreenOnionFlg = false;
                FuncPlaySE();
            }
            if (MentaikoFlg)
            {
                MentaikoFlg = false;
                FuncPlaySE();
            }
            _pickupUseFlg = true;
            ++ResetCount;
            RequestSerialization();
        }
    }

    public void MainPickupUseUp()
    {
        _pickupUseFlg = false;
    }

    void OnTriggerEnter(Collider coll)
    {
        Ochazuke_Pickup os = coll.GetComponent<Ochazuke_Pickup>();

        if (os != null && MainCollState && Networking.LocalPlayer.IsOwner(gameObject))
        {
            if (os._main.TeaState && os._main.FrikakeState && (!NormalFlg && !GreenOnionFlg && !MentaikoFlg))
            {
                if (os._main.TypeNo == 1 || os._main.TypeNo == 3)
                {
                    NormalFlg = true;
                    os._main.FuncEat();
                }
                else if (os._main.TypeNo == 2)
                {
                    GreenOnionFlg = true;
                    os._main.FuncEat();
                }
                else if (os._main.TypeNo == 4)
                {
                    MentaikoFlg = true;
                    os._main.FuncEat();
                }
                FuncPlaySE();
                RequestSerialization();
            }
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            MainCollState = MainCollState;
            SubCollState = SubCollState;
            NormalFlg = NormalFlg;
            GreenOnionFlg = GreenOnionFlg;
            MentaikoFlg = MentaikoFlg;
            RequestSerialization();
        }
    }

    public void FuncPlaySE()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlaySE));
    }

    public void PlaySE()
    {
        _as.Play();
    }

    public void FuncReset()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Reset));
    }

    public void Reset()
    {
        VRCPickup pickup = (VRCPickup)_sub.gameObject.GetComponent(typeof(VRCPickup));
        if (pickup != null)
        {
            pickup.Drop();
        }
        _sub.gameObject.transform.localPosition = Vector3.zero;
        _sub.gameObject.transform.localRotation = Quaternion.identity;
        MainCollState = false;
        SubCollState = false;
        NormalFlg = false;
        GreenOnionFlg = false;
        MentaikoFlg = false;
        RequestSerialization();
    }
}
