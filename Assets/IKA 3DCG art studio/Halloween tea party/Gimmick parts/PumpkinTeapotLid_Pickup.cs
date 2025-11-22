
using UdonSharp;
using UnityEngine;
using UnityEngine.Animations;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class PumpkinTeapotLid_Pickup : UdonSharpBehaviour
{
    public PumpkinTeapotLid_PickupMain _main;

    public override void OnPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

        _main.MainPickup();
    }

    public override void OnPickupUseDown()
    {
        _main.MainPickupUseDown();
    }

}
