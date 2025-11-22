
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RealCharger_PickupSub : UdonSharpBehaviour
{
    public RealCharger_PickupMain _main;

    public override void OnPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(MainPickup));
    }

    public void MainPickup()
    {
        _main.MainPickup();
    }

    public override void OnDrop()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(MainDrop));
    }

    public void MainDrop()
    {
        _main.MainDrop();
    }

    public override void OnPickupUseDown()
    {
        Debug.Log($"OnPickupUseDown");
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(MainPickupUseDown));
    }

    public void MainPickupUseDown()
    {
        Debug.Log($"MainPickupUseDown");
        _main.MainPickupUseDown();
    }
}
