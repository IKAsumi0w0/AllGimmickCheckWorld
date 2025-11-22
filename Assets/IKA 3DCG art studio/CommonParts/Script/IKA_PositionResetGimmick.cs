
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class IKA_PositionResetGimmick : UdonSharpBehaviour
{
    public bool _dynamicParentFlg = true;
    [SerializeField] string _text = "3 use position reset";
    [SerializeField] Transform _parent;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(PickupFlg))] bool _pickupFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ResetCount))] int _resetCount = 0;

    public bool PickupFlg
    {
        get => _pickupFlg;
        set
        {
            _pickupFlg = value;
            if (_pickupFlg) this.transform.parent = null;
            else this.transform.parent = _parent;
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
            }
        }
    }

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (_dynamicParentFlg) PickupFlg = true;
        ResetCount = 0;
    }

    public override void OnPickupUseDown()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            ++ResetCount;
        }
    }

    public void Reset()
    {
        VRCPickup pickup0 = (VRCPickup)this.GetComponent(typeof(VRCPickup));
        if (pickup0 != null)
        {
            pickup0.Drop();
        }
        if (_dynamicParentFlg) PickupFlg = false;
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
        ResetCount = 0;
    }
}
