
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class SBTSet_Toast01 : UdonSharpBehaviour
{
    [SerializeField] GameObject _setspreadObj0;
    [SerializeField] GameObject _unspreadObj0;
    [SerializeField] GameObject _setspreadObj1;
    [SerializeField] GameObject _unspreadObj1;
    [SerializeField] GameObject _setObj = default;
    [SerializeField] GameObject _unsetObj = default;
    [SerializeField] Transform _parent;
    [SerializeField] AudioSource _as;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(SetFlg))] bool _setFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(PickupFlg))] bool _posFlg = false;

    public bool SetFlg
    {
        get => _setFlg;
        set
        {
            _setFlg = value;
            _setspreadObj0.SetActive(_setFlg);
            _unspreadObj0.SetActive(!_setFlg);
            _setspreadObj1.SetActive(_setFlg);
            _unspreadObj1.SetActive(!_setFlg);
        }
    }

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
        SetFlg = false;
    }

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        PickupFlg = true;
    }

    public override void OnPickupUseDown()
    {
        SendCustomEventDelayedSeconds(nameof(Respawn), 0.5f, VRC.Udon.Common.Enums.EventTiming.Update);
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlaySe));
    }

    public void Respawn()
    {
        SetFlg = false;
        VRCPickup obj = (VRCPickup)gameObject.GetComponent(typeof(VRCPickup));
        if (obj != null)
        {
            obj.Drop();
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        PickupFlg = false;
    }

    public void PlaySe()
    {
        _as.Play();
    }
}
