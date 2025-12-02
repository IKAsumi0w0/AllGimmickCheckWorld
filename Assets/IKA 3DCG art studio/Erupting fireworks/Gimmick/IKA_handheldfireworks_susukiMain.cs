
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IKA_handheldfireworks_susukiMain : UdonSharpBehaviour
{
    [SerializeField] private GameObject _psObj;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(TogglePsObj))] private bool _flg = false;

    public bool TogglePsObj
    {
        get => _flg;
        set
        {
            _flg = value;
            _psObj.SetActive(_flg);
        }
    }

    public void MainPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    public void MainPickupUseDown()
    {
        TogglePsObj = !TogglePsObj;
        RequestSerialization();
    }
}
