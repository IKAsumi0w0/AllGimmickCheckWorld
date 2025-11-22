
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BambooLeafPlate_Gimmick : UdonSharpBehaviour
{
    public RiceSet_Manager _setM;
    public int _plateNo;
    public BambooLeafPlate_Pickup _bpp;
    public Transform _setPosTrans;
    [SerializeField] BoxCollider _collSub;
    [SerializeField] BoxCollider _collMain;
    [SerializeField] MeshRenderer _mr;
    float _timer = 0f;
    float _resetDelay = 5f;
    int _resetCount = 0;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(MainCollState))] bool _mainCollState = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(SubCollState))] bool _subCollState = false;

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
        for (int i = 0; i < _setM._rbm._objs.Length; i++)
        {
            if (_setM._rbm._objs[i]._main.PlateNo == _plateNo)
            {
                if (!Networking.LocalPlayer.IsOwner(_setM._rbm._objs[i]._main.gameObject)) Networking.SetOwner(Networking.LocalPlayer, _setM._rbm._objs[i]._main.gameObject);
                if (!Networking.LocalPlayer.IsOwner(_setM._rbm._objs[i].gameObject)) Networking.SetOwner(Networking.LocalPlayer, _setM._rbm._objs[i].gameObject);
            }
        }
    }

    public void MainDrop()
    {

    }

    public void MainPickupUseDown()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            ++ResetCount;
            _timer = 0;
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            MainCollState = MainCollState;
            SubCollState = SubCollState;
            RequestSerialization();
        }
    }

    public void Reset()
    {
        for (int i = 0; i < _setM._rbm._objs.Length; i++)
        {
            if (_setM._rbm._objs[i]._main.PlateNo == _plateNo)
            {
                if (!Networking.LocalPlayer.IsOwner(_setM._rbm._objs[i]._main.gameObject)) Networking.SetOwner(Networking.LocalPlayer, _setM._rbm._objs[i]._main.gameObject);
                _setM._rbm._objs[i]._main.Reset();
            }
        }
        VRCPickup pickup1 = (VRCPickup)_bpp.gameObject.GetComponent(typeof(VRCPickup));
        if (pickup1 != null)
        {
            pickup1.Drop();
        }
        _bpp.gameObject.transform.localPosition = Vector3.zero;
        _bpp.gameObject.transform.localRotation = Quaternion.identity;
        MainCollState = false;
        SubCollState = false;
        RequestSerialization();
    }
}
