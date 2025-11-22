
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ChocolateFountain_PickupMain : UdonSharpBehaviour
{
    public string _type = "";
    public float _height = 0;
    public ChocolateFountain_PickupSub _sub;
    [SerializeField] VRC_Pickup _pickup;
    [SerializeField] BoxCollider _collSub;
    [SerializeField] BoxCollider _collMain;
    [SerializeField] MeshRenderer _mr;
    float _timer0 = 0f;
    float _resetDelay0 = 5f;
    float _timer1 = 0f;
    float _resetDelay1 = 30f;

    bool _meshRFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(PickupFlg))] bool _pickupFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ResetCount))] int _resetCount = 0;

    public int ResetCount
    {
        get => _resetCount;
        set
        {
            _resetCount = value;
            if (3 <= _resetCount)
            {
                ResetSub();
                _resetCount = 0;
            }
        }
    }

    public bool PickupFlg
    {
        get => _pickupFlg;
        set
        {
            _pickupFlg = value;
        }
    }

    public bool MeshRFlg
    {
        get => _meshRFlg;
        set
        {
            _meshRFlg = value;
            _mr.enabled = _meshRFlg;
            _collSub.enabled = _meshRFlg;
            _collMain.enabled = _meshRFlg;
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
                _timer0 += Time.deltaTime;
                if (_resetDelay0 <= _timer0)
                {
                    ResetCount = 0;
                    RequestSerialization();
                    _timer0 = 0f;
                }
            }

            if (PickupFlg && !_pickup.IsHeld)
            {
                _timer1 += Time.deltaTime;
                if (_resetDelay1 <= _timer1)
                {
                    ResetCount = 3;
                    RequestSerialization();
                    _timer1 = 0f;
                }
            }
        }
    }

    public void MainPickup()
    {
        FuncMeshR_ON();
        _timer1 = 0f;
        PickupFlg = true;
    }

    public void MainDrop()
    {

    }

    public void MainPickupUseDown()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            ++ResetCount;
            _timer0 = 0;
            RequestSerialization();
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            if (MeshRFlg) FuncMeshR_ON();
            else FuncMeshR_OFF();
        }
    }

    public void FuncMeshR_ON()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(FuncMeshR_ONSub));
    }

    public void FuncMeshR_ONSub() { MeshRFlg = true; }

    public void FuncMeshR_OFF()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(FuncMeshR_OFFSub));
    }

    public void FuncMeshR_OFFSub() { MeshRFlg = false; }

    public void Reset()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ResetSub));
    }

    public void ResetSub()
    {
        VRCPickup pickup1 = (VRCPickup)_sub.gameObject.GetComponent(typeof(VRCPickup));
        if (pickup1 != null)
        {
            pickup1.Drop();
        }
        FuncMeshR_OFF();
        ResetCount = 0;
        PickupFlg = false;
        _sub.gameObject.transform.localPosition = Vector3.zero;
        _sub.gameObject.transform.localRotation = Quaternion.identity;
        RequestSerialization();
    }
}
