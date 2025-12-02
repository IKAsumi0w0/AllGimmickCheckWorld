
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IKA_PickupObj_OnOffSwitch : UdonSharpBehaviour
{
    [SerializeField] private GameObject _obj;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ToggleObj))] private bool _flg = false;

    public bool ToggleObj
    {
        get => _flg;
        set
        {
            _flg = value;
            _obj.SetActive(_flg);
        }
    }

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        VRCPlayerApi player = Networking.LocalPlayer;
        VRC_Pickup vRC_Pickup = (VRC_Pickup)this.gameObject.GetComponent(typeof(VRC_Pickup));
        if (player.IsUserInVR())
        {
            vRC_Pickup.orientation = VRC_Pickup.PickupOrientation.Any;
        }
        else
        {
            vRC_Pickup.orientation = VRC_Pickup.PickupOrientation.Grip;
        }
    }

    public override void OnDrop()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SwitchOff));
    }

    public override void OnPickupUseDown()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            SwitchOnOff();
        }
        else
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(SwitchOnOff));
        }
    }

    public void SwitchOnOff()
    {
        ToggleObj = !ToggleObj;
        RequestSerialization();
    }

    public void SwitchOff()
    {
        ToggleObj = false;
        RequestSerialization();
    }
}
