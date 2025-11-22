
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SakuraJuice_PickupMain : UdonSharpBehaviour
{
    public SakuraJuice_PickupSub _sub;
    [SerializeField] protected MeshRenderer _meshR;
    [SerializeField] protected Collider _mainColl;
    [SerializeField] protected Collider _subColl;
    float _timer0 = 0f;
    float _resetDelay0 = 5f;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(DisplayFlg))] protected bool _displayFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ResetCount))] int _resetCount = 0;

    public bool DisplayFlg
    {
        get => _displayFlg;
        set
        {
            _displayFlg = value;
            if (_meshR) _meshR.enabled = _displayFlg;
            _mainColl.enabled = _displayFlg;
            _subColl.enabled = _displayFlg;
        }
    }

    public int ResetCount
    {
        get => _resetCount;
        set
        {
            _resetCount = value;
            if (3 <= _resetCount)
            {
                Reset();
                _resetCount = 0;
            }
        }
    }

    void Update()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            if (0 < ResetCount)
            {
                _timer0 += Time.deltaTime;
                if (_resetDelay0 <= _timer0)
                {
                    ResetCount = 0;
                    RequestSerialization();
                    _timer0 = 0f;
                }
            }
        }
    }

    public void MainPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    public void MainPickupUseDown()
    {
        _timer0 = 0;
        ++ResetCount;
        Reset();
    }

    public void FuncDisplayFlg_ON() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(FuncDisplayFlg_ONSub));

    public void FuncDisplayFlg_ONSub()
    {
        DisplayFlg = true;
        RequestSerialization();
    }

    public void FuncDisplayFlg_OFF() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(FuncDisplayFlg_OFFSub));

    public void FuncDisplayFlg_OFFSub()
    {
        DisplayFlg = false;
        RequestSerialization();
    }

    public void Reset()
    {
        VRCPickup p = _sub.GetComponent<VRCPickup>();
        if (p != null)
        {
            p.Drop();
        }
        DisplayFlg = false;
        ResetCount = 0;
        _sub.transform.localPosition = Vector3.zero;
        _sub.transform.localRotation = Quaternion.identity;
        RequestSerialization();
    }
}
