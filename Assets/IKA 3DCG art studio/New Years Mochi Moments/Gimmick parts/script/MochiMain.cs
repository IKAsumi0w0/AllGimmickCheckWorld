
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;
using UnityEngine.UI;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MochiMain : UdonSharpBehaviour
{
    public BoxCollider _coll;
    public BoxCollider _subColl;
    public MeshRenderer _mochiMeshR;
    [SerializeField] SkinnedMeshRenderer _smr = default;
    [SerializeField] Mochi _sub = default;
    [SerializeField] int _probability = 6;
    [SerializeField] GameObject _burntObj = default;
    [SerializeField] GameObject _otosidamaMainObj = default;
    [SerializeField] GameObject _otosidamaObj1 = default;
    [SerializeField] GameObject _otosidamaObj2 = default;
    [SerializeField] Text _ui = default;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(FlgSwitchInt))] int _flgSwitchInt = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ObjSwitchInt))] int _objSwitchInt = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(GrilledFloat))] float _grilledFloat = -50f;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ResetFlg))] bool _rFlg = false;

    public int FlgSwitchInt
    {
        get => _flgSwitchInt;
        set
        {
            _flgSwitchInt = value;
        }
    }

    public int ObjSwitchInt
    {
        get => _objSwitchInt;
        set
        {
            _objSwitchInt = value;
        }
    }

    public float GrilledFloat
    {
        get => _grilledFloat;
        set
        {
            _grilledFloat = value;
            if (100f <= _grilledFloat)
            {
                _grilledFloat = 100f;
            }
            _smr.SetBlendShapeWeight(0, _grilledFloat);
            if (100 <= _grilledFloat && FlgSwitchInt == 0)
            {
                _smr.enabled = false;
                _mochiMeshR.enabled = false;
                _burntObj.SetActive(true);
                _otosidamaObj1.SetActive(false);
                _otosidamaObj2.SetActive(false);
            }
            else if (100 <= _grilledFloat && FlgSwitchInt == 1)
            {
                _smr.enabled = false;
                _mochiMeshR.enabled = false;
                _burntObj.SetActive(false);
                _otosidamaMainObj.SetActive(true);
                if (ObjSwitchInt == 0) _otosidamaObj1.SetActive(true);
                else _otosidamaObj2.SetActive(true);
            }
            else if (0 <= _grilledFloat)
            {
                _smr.enabled = true;
                _mochiMeshR.enabled = false;
                _burntObj.SetActive(false);
                _otosidamaObj1.SetActive(false);
                _otosidamaObj2.SetActive(false);
                if (Vector3.Dot(Vector3.up, _sub.transform.up) < 0)
                {
                    Vector3 upDir = _sub.transform.up;
                    _sub.transform.up = new Vector3(upDir.x, -upDir.y, upDir.z);
                }
            }
        }
    }

    public bool ResetFlg
    {
        get => _rFlg;
        set
        {
            _rFlg = value;
            HideMochi();
            //Debug.Log(_rFlg);
            //if (_rFlg) _mochiPack.Return(this.gameObject.transform.GetChild(0).gameObject);
        }
    }

    void Start()
    {
        FlgSwitchInt = Random.Range(0, 6);
        ObjSwitchInt = Random.Range(0, 2);
    }

    void OnTriggerStay(Collider c)
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            if (c.gameObject.layer == 17)
            {
                if (GrilledFloat < 100)
                {
                    if (FlgSwitchInt == 0) GrilledFloat += 0.5f;
                    else GrilledFloat += 0.25f;
                    RequestSerialization();
                }
            }
        }
    }

    public void MainPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    public void MainDrop()
    {
        SetGravity();
    }

    public void MainPickupUseDown()
    {
        if (100 <= GrilledFloat)
        {
            ResetFlg = true;
            RequestSerialization();
        }
    }

    public void SetGravity()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SetGravitySub));
    }

    public void SetGravitySub()
    {
        _sub._rigi.useGravity = true;
        _sub._rigi.isKinematic = false;
    }

    public void ShowMochi()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ShowMochiSub));
    }

    public void ShowMochiSub()
    {
        _coll.enabled = true;
        _subColl.enabled = true;
        _smr.enabled = false;
        _mochiMeshR.enabled = true;
        _sub._rigi.useGravity = true;
        _sub._rigi.isKinematic = false;
        _sub._rigi.velocity = Vector3.zero;
        _sub._rigi.angularVelocity = Vector3.zero;
    }

    public void HideMochi()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(HideMochiSub));
    }

    public void HideMochiSub()
    {
        VRCPickup pickup = (VRCPickup)_sub.GetComponent(typeof(VRCPickup));
        if (pickup != null) pickup.Drop();
        GrilledFloat = -50f;
        _smr.enabled = false;
        _burntObj.SetActive(false);
        _otosidamaMainObj.SetActive(false);
        _otosidamaObj1.SetActive(false);
        _otosidamaObj2.SetActive(false);
        _sub.transform.localPosition = Vector3.zero;
        _sub.transform.localRotation = Quaternion.identity;
        _coll.enabled = false;
        _subColl.enabled = false;
        _smr.enabled = false;
        _mochiMeshR.enabled = false;
        _sub._rigi.useGravity = false;
        _sub._rigi.isKinematic = true;
        _sub._rigi.velocity = Vector3.zero;
        _sub._rigi.angularVelocity = Vector3.zero;
    }

    public void Debug1()
    {
        _ui.text = $"SwitchInt:{FlgSwitchInt}\n" +
                   $"GrilledFloat:{GrilledFloat:F3}\n" +
                   $"ObjSwitchInt:{ObjSwitchInt}\n" +
                   $"_sub._rigi.isKinematic:{_sub._rigi.isKinematic}\n" +
                   $"_sub._rigi.useGravity:{_sub._rigi.useGravity}";
    }
}
