
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class BambooLeafPlate_Pickup : UdonSharpBehaviour
{
    public BambooLeafPlate_Gimmick _main;

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
}
