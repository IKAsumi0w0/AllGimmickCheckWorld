
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Teacup_Pickup : UdonSharpBehaviour
{
    public Teacup_Gimmick _main;

    public override void OnPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        _main.MainPickup();
    }

    public override void OnDrop()
    {
        _main.MainDrop();
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
