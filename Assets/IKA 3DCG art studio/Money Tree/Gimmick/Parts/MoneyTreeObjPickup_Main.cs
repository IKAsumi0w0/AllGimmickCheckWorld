
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MoneyTreeObjPickup_Main : UdonSharpBehaviour
{
    public MoneyTreeObjPickup_Sub _sub;
    [SerializeField] VRC_Pickup _pickup;
    [SerializeField] Rigidbody _rigi;
    [SerializeField] Collider _coll;
    [SerializeField] MeshRenderer _mr;
    bool _meshRFlg = false;

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

    public bool MeshRFlg
    {
        get => _meshRFlg;
        set
        {
            _meshRFlg = value;
            _mr.enabled = _meshRFlg;
            _coll.enabled = _meshRFlg;
            _rigi.useGravity = _meshRFlg;
            _rigi.isKinematic = !_meshRFlg;
            FuncResetRigi();
        }
    }

    public void MainPickup()
    {
        FuncGravityOFF();
    }

    public void MainDrop()
    {

    }

    public void MainPickupUseDown()
    {

    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            if (MeshRFlg) FuncMeshR_ON();
            else FuncMeshR_OFF();
        }
    }

    public void DelayFuncMeshR_ON()
    {
        SendCustomEventDelayedSeconds(nameof(FuncMeshR_ON), 0.2f, VRC.Udon.Common.Enums.EventTiming.Update);
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

    public void FuncGravityOFF()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(FuncGravityOFFSub));
    }

    public void FuncGravityOFFSub()
    {
        _rigi.useGravity = false;
        _rigi.isKinematic = true;
    }

    public void FuncResetRigi()
    {
        _rigi.velocity = Vector3.zero;
        _rigi.angularVelocity = Vector3.zero;
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
        _sub.gameObject.transform.localPosition = Vector3.zero;
        _sub.gameObject.transform.localRotation = Quaternion.identity;
        RequestSerialization();
    }
}
