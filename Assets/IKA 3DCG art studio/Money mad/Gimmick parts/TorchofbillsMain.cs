
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class TorchofbillsMain : UdonSharpBehaviour
{
    [SerializeField] GameObject _obj;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ToggleObj))] bool _flg = false;

    public bool ToggleObj
    {
        get => _flg;
        set
        {
            _flg = value;
            _obj.SetActive(_flg);
        }
    }

    public void MainPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        ToggleObj = true;
        RequestSerialization();
    }

    public void MainDrop()
    {
        ToggleObj = false;
        RequestSerialization();
    }

}
