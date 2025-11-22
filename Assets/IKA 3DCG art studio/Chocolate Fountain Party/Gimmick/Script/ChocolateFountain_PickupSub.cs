
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ChocolateFountain_PickupSub : UdonSharpBehaviour
{
    public ChocolateFountain_PickupMain _main;

    public override void OnPickup()
    {
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
