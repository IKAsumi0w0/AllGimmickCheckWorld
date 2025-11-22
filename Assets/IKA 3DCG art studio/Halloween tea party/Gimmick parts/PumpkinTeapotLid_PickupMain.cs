
using UdonSharp;
using UnityEngine;
using UnityEngine.Animations;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PumpkinTeapotLid_PickupMain : UdonSharpBehaviour
{
    public PumpkinTeapotLid_Pickup _sub;

    [SerializeField] ParentConstraint _teaPotLid;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(PickupFlg))] bool _pickupFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ResetCount))] int _resetCount = 0;

    public bool PickupFlg
    {
        get => _pickupFlg;
        set
        {
            _pickupFlg = value;
            _teaPotLid.constraintActive = !_pickupFlg;
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

    public virtual void MainPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        PickupFlg = true;
        ResetCount = 0;
        RequestSerialization();
    }

    public virtual void MainPickupUseDown()
    {
        ++ResetCount;
        RequestSerialization();
    }

    public void Reset()
    {
        VRCPickup pickup0 = (VRCPickup)_sub.GetComponent(typeof(VRCPickup));
        if (pickup0 != null)
        {
            pickup0.Drop();
        }
        PickupFlg = false;
        ResetCount = 0;
        RequestSerialization();
    }
}
