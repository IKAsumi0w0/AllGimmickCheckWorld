
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VR_mamemaki_coffee : UdonSharpBehaviour
{
    [SerializeField] GameObject _magazineObj;
    [SerializeField] GameObject _pickupObj;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(PickupFlg))] public bool _pickupFlg = false;

    public bool PickupFlg
    {
        get => _pickupFlg;
        set
        {
            _pickupFlg = value;
            _pickupObj.SetActive(_pickupFlg);
        }
    }

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        Networking.SetOwner(Networking.LocalPlayer, _pickupObj);
        VRCPlayerApi player = Networking.LocalPlayer;
        VRC_Pickup vRC_Pickup = (VRC_Pickup)this.gameObject.GetComponent(typeof(VRC_Pickup));
        if (player.IsUserInVR())
        {
            PickupFlg = true;
            vRC_Pickup.orientation = VRC_Pickup.PickupOrientation.Any;
            _pickupObj.transform.localPosition = Vector3.zero;

        }
        else
        {
            PickupFlg = false;
            vRC_Pickup.orientation = VRC_Pickup.PickupOrientation.Grip;
        }
    }

    public override void OnDrop()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        if (player.IsUserInVR())
        {
            PickupFlg = false;
        }
        _pickupObj.transform.localPosition = Vector3.zero;
    }

    public override void OnPickupUseDown()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        if (!player.IsUserInVR())
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Ignition));
        }
    }

    public void Ignition()
    {
        for (int i = 0; i < _magazineObj.transform.childCount; i++)
        {
            if (!_magazineObj.transform.GetChild(i).gameObject.activeSelf)
            {
                _magazineObj.transform.GetChild(i).gameObject.SetActive(true);
                return;
            }
        }
    }
}
