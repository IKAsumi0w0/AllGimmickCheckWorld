
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CRTTV_Remotecontrol : UdonSharpBehaviour
{
    [SerializeField] CRTTV_Gimmick _tvObj;

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        Networking.SetOwner(Networking.LocalPlayer, _tvObj.gameObject);
    }

    public override void OnPickupUseDown()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            RemoteSwitch();
        }
        else
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(RemoteSwitch));
        }
    }

    public void RemoteSwitch()
    {
        _tvObj.ShowSwitch();
    }
}
