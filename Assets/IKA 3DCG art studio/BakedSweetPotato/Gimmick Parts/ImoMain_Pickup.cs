
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class ImoMain_Pickup : UdonSharpBehaviour
{
    [SerializeField] WoodenStickMain _woodenStickMain;


    public override void OnPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(TrueImoMainPickupState));
    }

    public void TrueImoMainPickupState()
    {
        _woodenStickMain.SEOff();
        if (Networking.LocalPlayer.IsOwner(_woodenStickMain.gameObject)) _woodenStickMain.TrueImoMainPickupState();
    }

    public override void OnPickupUseDown()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        _woodenStickMain.TrueImoSubDisplay();
    }

}
