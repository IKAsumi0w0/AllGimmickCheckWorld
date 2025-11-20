
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class IKA_HandFireworksSusuki : UdonSharpBehaviour
{
    [SerializeField] private GameObject _psObj;
    IKA_FireworksIgnition _ikaFwI;
    IKA_HandFireworksSusuki _ikaFwIS;
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

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    public override void OnPickupUseDown()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            IgnitionExtinguishSwitch();
        }
        else
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(IgnitionExtinguishSwitch));
        }
    }

    public void IgnitionExtinguishSwitch()
    {
        if (TogglePsObj) TogglePsObj = false;
        else TogglePsObj = true;
    }

    void OnTriggerStay(Collider other)
    {
        if (TogglePsObj)
        {
            _ikaFwI = other.GetComponent<IKA_FireworksIgnition>();
            if (_ikaFwI != null)
            {
                _ikaFwI.TogglePsObj = true;
            }
            _ikaFwIS = other.GetComponent<IKA_HandFireworksSusuki>();
            if (_ikaFwIS != null)
            {
                _ikaFwIS.TogglePsObj = true;
            }
        }
    }
}
