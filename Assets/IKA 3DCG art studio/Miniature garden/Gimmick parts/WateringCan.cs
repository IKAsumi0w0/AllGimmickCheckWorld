
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class WateringCan : UdonSharpBehaviour
{
    [SerializeField] GameObject _psObj;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(PsSwitch))] private bool _flg = false;

    public bool PsSwitch
    {
        get => _flg;
        set
        {
            _flg = value;
            _psObj.SetActive(_flg);
        }
    }

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);

    }

    public override void OnPickupUseDown()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            FlgSwitch();
        }
        else
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(FlgSwitch));
        }
    }

    public void FlgSwitch()
    {
        if (PsSwitch) PsSwitch = false;
        else  PsSwitch = true;
    }
}
