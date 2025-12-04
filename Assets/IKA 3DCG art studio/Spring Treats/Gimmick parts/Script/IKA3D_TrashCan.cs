
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IKA3D_TrashCan : UdonSharpBehaviour
{
    [SerializeField] Collider _coll;
    [SerializeField] TrashCan_SpringTreats _trashCan_SpringTreats;

    public override void Interact()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (!Networking.LocalPlayer.IsOwner(_trashCan_SpringTreats.gameObject)) Networking.SetOwner(Networking.LocalPlayer, _trashCan_SpringTreats.gameObject);
        //_coll.enabled = true;
        //SendCustomEventDelayedSeconds(nameof(HideColl), 0.1f);
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ShowColl));
    }

    public void ShowColl()
    {
        _coll.enabled = true;
        SendCustomEventDelayedSeconds(nameof(HideColl), 0.1f);
    }

    public void HideColl()
    {
        _coll.enabled = false;
    }
}
