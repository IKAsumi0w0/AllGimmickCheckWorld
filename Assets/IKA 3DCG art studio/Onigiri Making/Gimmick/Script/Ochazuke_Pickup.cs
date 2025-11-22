
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Ochazuke_Pickup : UdonSharpBehaviour
{
    public Ochazuke_Gimmick _main;

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
