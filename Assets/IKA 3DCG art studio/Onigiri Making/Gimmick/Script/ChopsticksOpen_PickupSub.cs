
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ChopsticksOpen_PickupSub : UdonSharpBehaviour
{
    public ChopsticksOpen_Pickup _main;

    public override void OnPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        _main.MainPickup();
    }

    public override void OnPickupUseDown()
    {
        _main.MainPickupUseDown();
    }

    public override void OnPickupUseUp()
    {
        _main.MainPickupUseUp();
    }
}
