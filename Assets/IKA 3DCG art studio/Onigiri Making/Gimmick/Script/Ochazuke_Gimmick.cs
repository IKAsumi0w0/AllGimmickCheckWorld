
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Ochazuke_Gimmick : UdonSharpBehaviour
{
    [SerializeField] Ochazuke_Pickup _sub;
    [SerializeField] BoxCollider _collSub;
    [SerializeField] BoxCollider _collMain;
    [SerializeField] MeshRenderer _mr;
    [SerializeField] VRCPickup _vRCPickup;
    [SerializeField] GameObject _tea;
    [SerializeField] GameObject _furikake;
    [SerializeField] GameObject _normal;
    [SerializeField] GameObject _greenOnion;
    [SerializeField] GameObject _ume;
    [SerializeField] GameObject _mentai;
    float _timer = 0f;
    float _resetDelay = 1f;
    int _resetCount = 0;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(MainCollState))] bool _mainCollState = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(SubCollState))] bool _subCollState = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(TeaState))] bool _teaState = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(FrikakeState))] bool _frikakeState = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(TypeNo))] int _typeNo = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(EatCount))] int _eatCount = 0;

    public bool MainCollState
    {
        get => _mainCollState;
        set
        {
            _mainCollState = value;
            _collMain.enabled = value;
        }
    }

    public bool SubCollState
    {
        get => _subCollState;
        set
        {
            _subCollState = value;
            _mr.enabled = value;
            _collSub.enabled = value;
        }
    }

    public bool TeaState
    {
        get => _teaState;
        set
        {
            _teaState = value;
            _tea.SetActive(value);
        }
    }

    public bool FrikakeState
    {
        get => _frikakeState;
        set
        {
            _frikakeState = value;
            _furikake.SetActive(value);
        }
    }

    public int TypeNo
    {
        get => _typeNo;
        set
        {
            _typeNo = value;
            _normal.SetActive(false);
            _greenOnion.SetActive(false);
            _ume.SetActive(false);
            _mentai.SetActive(false);
            switch (_typeNo)
            {
                case 0:

                    break;

                case 1:
                    _normal.SetActive(true);
                    break;

                case 2:
                    _greenOnion.SetActive(true);
                    break;

                case 3:
                    _ume.SetActive(true);
                    break;

                case 4:
                    _mentai.SetActive(true);
                    break;

                default:

                    break;
            }
        }
    }

    public int EatCount
    {
        get => _eatCount;
        set
        {
            _eatCount = value;
            if (Networking.LocalPlayer.IsOwner(gameObject) && 4 < _eatCount)
            {
                TeaState = false;
                FrikakeState = false;
                TypeNo = 0;
                EatCount = 0;
                RequestSerialization();
            }
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
        ResetCount = 0;
    }

    public void MainPickupUseDown()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            ++ResetCount;
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            //おにぎり
            ShapedRiceBall_Gimmick oni = coll.GetComponent<ShapedRiceBall_Gimmick>();
            if (oni != null)
            {
                if (oni.TypeNo == 2)
                {
                    TypeNo = 1;
                    oni.Reset();
                }
                else if (oni.TypeNo == 3)
                {
                    TypeNo = 2;
                    oni.Reset();
                }
                else if (oni.TypeNo == 4)
                {
                    TypeNo = 3;
                    oni.Reset();
                }
                else if (oni.TypeNo == 10)
                {
                    TypeNo = 4;
                    oni.Reset();
                }
                RequestSerialization();
            }
        }
    }

    public void FuncEat()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(FuncEatSub));
    }

    public void FuncEatSub()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            ++EatCount;
            RequestSerialization();
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            MainCollState = MainCollState;
            SubCollState = SubCollState;
            TeaState = TeaState;
            FrikakeState = FrikakeState;
            RequestSerialization();
        }
    }

    public void Reset()
    {
        _vRCPickup.Drop();
        _sub.gameObject.transform.localPosition = Vector3.zero;
        _sub.gameObject.transform.localRotation = Quaternion.identity;
        MainCollState = false;
        SubCollState = false;
        TeaState = false;
        FrikakeState = false;
        TypeNo = 0;
        EatCount = 0;
        RequestSerialization();
    }
}
