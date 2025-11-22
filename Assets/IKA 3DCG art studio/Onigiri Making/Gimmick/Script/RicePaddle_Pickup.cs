
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RicePaddle_Pickup : UdonSharpBehaviour
{
    public RicePaddle_Gimmick _main;

    public override void OnPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        _main.MainPickup();
    }

    public override void OnPickupUseDown()
    {
        _main.MainPickupUseDown();
    }
}
