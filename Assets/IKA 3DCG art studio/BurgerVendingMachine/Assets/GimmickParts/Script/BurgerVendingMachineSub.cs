
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BurgerVendingMachineSub : UdonSharpBehaviour
{
    public UdonSharpBehaviour _main;

    public override void Interact()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (_main != null && !Networking.LocalPlayer.IsOwner(_main.gameObject)) Networking.SetOwner(Networking.LocalPlayer, _main.gameObject);

        if (_main != null)
        {
            _main.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "InteractMain");
        }
    }
}
