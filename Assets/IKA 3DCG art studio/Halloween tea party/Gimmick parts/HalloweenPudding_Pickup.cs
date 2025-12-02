
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class HalloweenPudding_Pickup : UdonSharpBehaviour
{
    [SerializeField] Transform _resetPos;
    [SerializeField] HalloweenSpoon_Pickup _spoon;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ResetCount))] int _resetCount = 0;

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
        Networking.SetOwner(Networking.LocalPlayer, _spoon.gameObject);
        ResetCount = 0;
    }

    public override void OnPickupUseDown()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            ++ResetCount;
            RequestSerialization();
        }
    }

    public void Reset()
    {
        VRCPickup pickup0 = (VRCPickup)_spoon.GetComponent(typeof(VRCPickup));
        if (pickup0 != null)
        {
            pickup0.Drop();
        }
        _spoon.transform.position = _resetPos.position;
        _spoon.transform.rotation = _resetPos.rotation;
        ResetCount = 0;
        RequestSerialization();
    }
}
