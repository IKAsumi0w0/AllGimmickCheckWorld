
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CRTTV_Remotecontrol : UdonSharpBehaviour
{
    [SerializeField] CRTTV_Gimmick _tvObj;

    public override void OnPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (!Networking.LocalPlayer.IsOwner(_tvObj.gameObject)) Networking.SetOwner(Networking.LocalPlayer, _tvObj.gameObject);
    }

    public override void OnPickupUseDown()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (!Networking.LocalPlayer.IsOwner(_tvObj.gameObject)) Networking.SetOwner(Networking.LocalPlayer, _tvObj.gameObject);
        _tvObj.ModelSwitch = !_tvObj.ModelSwitch;
        _tvObj.RequestSerialization();
    }
}
