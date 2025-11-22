
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CRTTV_Gimmick : UdonSharpBehaviour
{
    [SerializeField] GameObject _offObj;
    [SerializeField] GameObject _onObj;
    [SerializeField] GameObject _onScreenObj;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ModelSwitch))] bool _flg = false;

    public bool ModelSwitch
    {
        get => _flg;
        set
        {
            _flg = value;
            _offObj.SetActive(!_flg);
            _onObj.SetActive(_flg);
            _onScreenObj.SetActive(_flg);
        }
    }

    public override void Interact()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            ShowSwitch();
        }
        else
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(ShowSwitch));
        }
    }

    public void ShowSwitch()
    {
        if (ModelSwitch)
        {
            ModelSwitch = false;
        }
        else
        {
            ModelSwitch = true;
        }
    }
}
