
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class IKA3D_SpwnGimmickBase_PickupSub : UdonSharpBehaviour
{
    public IKA3D_SpwnGimmickBase_PickupMain _main;

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
