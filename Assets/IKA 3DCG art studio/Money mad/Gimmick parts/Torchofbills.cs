
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Torchofbills : UdonSharpBehaviour
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
        ToggleObj = true;
    }

    public override void OnDrop()
    {
        ToggleObj = false;
    }
}
