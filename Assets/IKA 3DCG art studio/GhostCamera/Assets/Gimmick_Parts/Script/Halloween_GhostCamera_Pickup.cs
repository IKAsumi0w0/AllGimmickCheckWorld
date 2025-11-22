
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Halloween_GhostCamera_Pickup : UdonSharpBehaviour
{
    [Header("転送先（Halloween_GhostCamera_Main を指定）")]
    public Halloween_GhostCamera_Main _main;

    public override void OnPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (_main != null && !Networking.LocalPlayer.IsOwner(_main.gameObject)) Networking.SetOwner(Networking.LocalPlayer, _main.gameObject);
    }

    public override void OnPickupUseDown()
    {
        if (_main != null) _main.Shoot();
    }
}
