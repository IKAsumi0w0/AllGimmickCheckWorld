
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Umbrella_cherries : UdonSharpBehaviour
{
    [SerializeField] Transform _psPos;
    [SerializeField] GameObject _psObj;
    [SerializeField] float _psOffsetH;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ToggleObj))] private bool _flg = false;

    public bool ToggleObj
    {
        get => _flg;
        set
        {
            _flg = value;
            _psObj.SetActive(_flg);
        }
    }

    private void Update()
    {
        _psPos.position = this.gameObject.transform.position + new Vector3(0, _psOffsetH, 0);
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
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(SwitchOn));
    }

    public override void OnDrop()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(SwitchOff));
    }


    public void SwitchOn()
    {
        ToggleObj = true;
    }

    public void SwitchOff()
    {
        ToggleObj = false;
    }
}
