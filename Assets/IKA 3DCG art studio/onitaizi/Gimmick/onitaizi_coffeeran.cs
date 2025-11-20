
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class onitaizi_coffeeran : UdonSharpBehaviour
{
    [SerializeField] GameObject _magazineObj;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ShotFlg))] private bool _shotFlg = false;

    public bool ShotFlg
    {
        get => _shotFlg;
        set
        {
            _shotFlg = value;
            if (_shotFlg) SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Ignition));
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
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(IgnitionFalse));
    }

    public override void OnPickupUseDown()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(IgnitionTrue));
    }

    public override void OnPickupUseUp()
    {
        ShotFlg = false;
    }

    public void Ignition()
    {
        if (ShotFlg)
        {
            for (int i = 0; i < _magazineObj.transform.childCount; i++)
            {
                if (!_magazineObj.transform.GetChild(i).gameObject.activeSelf)
                {
                    _magazineObj.transform.GetChild(i).gameObject.SetActive(true);
                    break;
                }
            }
            SendCustomEventDelayedSeconds(nameof(Ignition), 0.5f, VRC.Udon.Common.Enums.EventTiming.Update);
        }
    }

    public void IgnitionTrue() { ShotFlg = true; }
    public void IgnitionFalse() { ShotFlg = false; }
}
