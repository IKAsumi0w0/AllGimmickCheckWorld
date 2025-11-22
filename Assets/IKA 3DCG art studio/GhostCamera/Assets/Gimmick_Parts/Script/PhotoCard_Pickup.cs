using UdonSharp;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using VRC.SDKBase;

public class PhotoCard_Pickup : UdonSharpBehaviour
{
    [Header("転送先（PhotoCardController を指定）")]
    public PhotoCardController _card;

    public override void OnPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

        if (_card != null && !Networking.LocalPlayer.IsOwner(_card.gameObject))
            Networking.SetOwner(Networking.LocalPlayer, _card.gameObject);

        if (_card != null)
        {
            // 全員のクライアントで Constraint を OFF
            _card.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "NetConstraintOff");
        }
    }
}
