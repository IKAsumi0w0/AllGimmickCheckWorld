
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IKA_Folding_fan_PickupObj_OnOffSwitchMain : UdonSharpBehaviour
{
    [SerializeField] GameObject _obj;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ObjState))] bool _state = false;

    public bool ObjState
    {
        get => _state;
        set
        {
            _state = value;
            _obj.SetActive(_state);
        }
    }

    public void MainPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    public void MainDrop()
    {
        if (ObjState)
        {
            ObjState = false;
            RequestSerialization();
        }
    }

    public void MainPickupUseDown()
    {
        ObjState = !ObjState;
        RequestSerialization();
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            ObjState = ObjState;
            RequestSerialization();
        }
    }

}
