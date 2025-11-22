
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ST_Spoon_PickupSub : UdonSharpBehaviour
{
    public ST_Spoon_PickupMain _main;

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
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(MainPickupUseDown));
    }

    public void MainPickupUseDown()
    {
        _main.MainPickupUseDown();
    }
}
