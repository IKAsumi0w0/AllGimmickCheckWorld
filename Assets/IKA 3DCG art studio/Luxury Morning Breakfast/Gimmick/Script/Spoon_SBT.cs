
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class Spoon_SBT : UdonSharpBehaviour
{
    [SerializeField] Transform _parent;
    [SerializeField] GameObject _setObj = default;
    [SerializeField] GameObject _unsetObj = default;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(PickupFlg))] bool _posFlg = false;

    public bool PickupFlg
    {
        get => _posFlg;
        set
        {
            _posFlg = value;
            if (_posFlg)
            {
                transform.parent = null;
                if (_setObj != null) _setObj.SetActive(false);
                if (_unsetObj != null) _unsetObj.SetActive(true);
            }
            else
            {
                transform.parent = _parent;
                if (_setObj != null) _setObj.SetActive(true);
                if (_unsetObj != null) _unsetObj.SetActive(false);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
        }
    }

    void Start()
    {
        PickupFlg = false;
    }

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        PickupFlg = true;
    }

    public override void OnPickupUseDown()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Reset));
    }

    public void Reset()
    {
        VRCPickup obj = (VRCPickup)transform.GetComponent(typeof(VRCPickup));
        if (obj != null)
        {
            obj.Drop();
        }
        PickupFlg = false;
    }
}
