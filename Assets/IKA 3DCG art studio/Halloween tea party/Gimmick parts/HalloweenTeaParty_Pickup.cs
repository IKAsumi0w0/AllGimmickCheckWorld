
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HalloweenTeaParty_Pickup : UdonSharpBehaviour
{
    [SerializeField] HalloweenTeaParty_Gimmick _main;

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
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
