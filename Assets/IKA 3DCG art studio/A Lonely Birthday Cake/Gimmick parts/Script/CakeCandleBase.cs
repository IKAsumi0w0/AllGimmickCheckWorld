using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class CakeCandleBase : UdonSharpBehaviour
{
    public WholeCake_PickupMain _wcpm;
    [SerializeField] protected GameObject _fireObj;
    [SerializeField] protected BoxCollider _coll;
    protected VRCPlayerApi _p;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(FireFlg))] protected bool _fireFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(DisplayFlg))] protected bool _displayFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ProtectionFlg))] protected bool _protectionFlg = false;

    public bool FireFlg
    {
        get => _fireFlg;
        set
        {
            _fireFlg = value;
            if (_fireObj != null)
            {
                _fireObj.SetActive(_fireFlg);
            }
        }
    }

    public bool DisplayFlg
    {
        get => _displayFlg;
        set
        {
            if (_displayFlg == value) return;

            _displayFlg = value;
            if (_coll != null) _coll.enabled = _displayFlg;
            OnDisplayFlgChanged();
        }
    }

    protected virtual void OnDisplayFlgChanged() { }

    public bool ProtectionFlg
    {
        get => _protectionFlg;
        set => _protectionFlg = value;
    }

    protected virtual void Start()
    {
        _p = Networking.LocalPlayer;
    }

    protected virtual void Update()
    {
        if (_p != null && DisplayFlg && !ProtectionFlg && FireFlg)
        {
            Vector3 headPosition = _p.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            float dis = Vector3.Distance(transform.position, headPosition);

            if (dis < 0.5f)
            {
                HideFire();
            }
        }
    }

    public void HideFire()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(HideFireSub));
    }

    public void HideFireSub()
    {
        FireFlg = false;
        RequestSerialization();
    }

    void OnTriggerEnter(Collider coll)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            IgnitionRod_PickupMain irpm = coll.GetComponent<IgnitionRod_PickupMain>();
            if (irpm != null && irpm.IgnitionFlg && !FireFlg)
            {
                FireFlg = true;
                RequestSerialization();
                ++_wcpm.LightingCount;
                _wcpm.UpdateValueSub();
            }
        }
    }

    public void UpdateValue()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(UpdateValueSub));
    }

    public void UpdateValueSub()
    {
        RequestSerialization();
    }

}
