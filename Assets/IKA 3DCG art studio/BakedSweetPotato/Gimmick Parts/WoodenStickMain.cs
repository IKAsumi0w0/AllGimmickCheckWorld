
using UdonSharp;
using UnityEngine;
using UnityEngine.Animations;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class WoodenStickMain : UdonSharpBehaviour
{
    [HideInInspector] public float _explosionProbability = 10;
    [Header("=====棒=====")]
    public GameObject _woodenStickObj;
    [SerializeField] MeshRenderer _woodenStickMR;
    [SerializeField] BoxCollider _woodenStickColl;
    [SerializeField] BoxCollider _hitWoodenStickColl;
    [Header("=====芋Main=====")]
    [SerializeField] GameObject _imoMainPickupObj;
    [SerializeField] MeshRenderer _imoMainMeshR;
    [SerializeField] BoxCollider _imoMainColl;
    [SerializeField] ParentConstraint _imoMainParentConstraint;
    [Header("=====芋Sub=====")]
    [SerializeField] GameObject _normalSE;
    [SerializeField] GameObject _bomSE;
    [SerializeField] Potato_01 _imoSub1;
    [SerializeField] Potato_02 _imoSub2;
    [SerializeField] ParentConstraint _imoSubParentConstraint;



    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(WoodenStickState))] bool _woodenStickState = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ExplosionState))] bool _explosionState = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ImoMainDisplayState))] bool _imoMainState = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ImoMainPickupState))] bool _imoMainPickupState = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ImoSubPickupState))] bool _imoSubPickupState = false;

    public bool WoodenStickState
    {
        get => _woodenStickState;
        set
        {
            _woodenStickState = value;
            if (_woodenStickMR) _woodenStickMR.enabled = _woodenStickState;
            if (_woodenStickColl) _woodenStickColl.enabled = _woodenStickState;
            if (_hitWoodenStickColl) _hitWoodenStickColl.enabled = _woodenStickState;
        }
    }

    public bool ExplosionState
    {
        get => _explosionState;
        set
        {
            _explosionState = value;
        }
    }

    public bool ImoMainDisplayState
    {
        get => _imoMainState;
        set
        {
            _imoMainState = value;
            _imoMainMeshR.enabled = _imoMainState;
            _imoMainColl.enabled = _imoMainState;
        }
    }

    public bool ImoMainPickupState
    {
        get => _imoMainPickupState;
        set
        {
            _imoMainPickupState = value;
            _imoMainParentConstraint.constraintActive = !_imoMainPickupState;
        }
    }

    public bool ImoSubPickupState
    {
        get => _imoSubPickupState;
        set
        {
            _imoSubPickupState = value;
            _imoSubParentConstraint.constraintActive = !value;
        }
    }

    #region 木の枝

    public void TrueWoodenStick() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(TrueWoodenStickSub));
    public void TrueWoodenStickSub()
    {
        WoodenStickState = true;
        RequestSerialization();
    }

    public void FalseWoodenStick() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(FalseWoodenStickSub));
    public void FalseWoodenStickSub()
    {
        WoodenStickState = false;
        RequestSerialization();
    }

    public void DropWoodenStick() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(DropWoodenStickSub));
    public void DropWoodenStickSub()
    {
        VRCPickup p = (VRCPickup)_woodenStickObj.GetComponent(typeof(VRCPickup));
        if (p != null)
        {
            p.Drop();
        }
    }

    #endregion

    #region 爆発

    public void SetExplosionState() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(SetExplosionStateSub));
    public void SetExplosionStateSub()
    {
        _explosionState = Random.Range(0, 100f) <= _explosionProbability;
        RequestSerialization();
    }

    #endregion


    #region 芋メイン

    public void TrueImoMainDisplay() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(TrueImoMainDisplaySub));
    public void TrueImoMainDisplaySub()
    {
        ImoMainDisplayState = true;
        RequestSerialization();
    }

    public void FalseImoMain() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(FalseImoMainSub));
    public void FalseImoMainSub()
    {
        ImoMainDisplayState = false;
        RequestSerialization();
    }

    public void TrueImoMainPickupState()
    {

        ImoMainPickupState = true;
        RequestSerialization();
    }

    public void FalseImoMainPickupState()
    {
        ImoMainPickupState = false;
        RequestSerialization();
    }

    public void DropImoMain() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(DropImoMainSub));
    public void DropImoMainSub()
    {
        VRCPickup p = (VRCPickup)_imoMainPickupObj.GetComponent(typeof(VRCPickup));
        if (p != null)
        {
            p.Drop();
        }
        FalseImoMainPickupState();
    }

    #endregion

    #region 芋サブ

    public void SEOff()
    {
        _bomSE.SetActive(false);
        _normalSE.SetActive(false);
    }

    public void TrueImoSubDisplay()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(TrueImoSubDisplay0));
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(TrueImoSubDisplay1));
    }

    public void TrueImoSubDisplay0()
    {
        DropWoodenStick();
        FalseWoodenStick();
        DropImoMain();
        FalseImoMain();
        TrueImoSubPickupState();
        Networking.SetOwner(Networking.LocalPlayer, _imoSub1.gameObject);
        Networking.SetOwner(Networking.LocalPlayer, _imoSub2.gameObject);
        if (ExplosionState)
        {
            _imoSub1.ImoSubDisplayState = 3;
            _imoSub2.ImoSubDisplayState = 3;
        }
        else
        {
            _imoSub1.ImoSubDisplayState = 1;
            _imoSub2.ImoSubDisplayState = 1;
        }

        _imoSub1.RequestSerialization();
        _imoSub2.RequestSerialization();
        RequestSerialization();
    }

    public void TrueImoSubDisplay1()
    {
        if (ExplosionState)
        {
            _bomSE.SetActive(true);
        }
        else
        {
            _normalSE.SetActive(true);
        }
    }

    public void FalseImoSubDisplay() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(FalseImoSubDisplay0));
    public void FalseImoSubDisplay0()
    {
        if (!Networking.LocalPlayer.IsOwner(_imoSub1.gameObject))
            Networking.SetOwner(Networking.LocalPlayer, _imoSub1.gameObject);
        if (!Networking.LocalPlayer.IsOwner(_imoSub2.gameObject))
            Networking.SetOwner(Networking.LocalPlayer, _imoSub2.gameObject);
        VRCPickup p2 = (VRCPickup)_imoSub1.GetComponent(typeof(VRCPickup));
        VRCPickup p3 = (VRCPickup)_imoSub2.GetComponent(typeof(VRCPickup));
        if (p2 != null) p2.Drop();
        if (p3 != null) p3.Drop();
        _imoSub1.ImoSubDisplayState = 0;
        _imoSub2.ImoSubDisplayState = 0;
        _imoSub1.RequestSerialization();
        _imoSub2.RequestSerialization();
        _imoSub1.transform.localPosition = Vector3.zero;
        _imoSub2.transform.localPosition = Vector3.zero;
        _bomSE.SetActive(false);
        _normalSE.SetActive(false);
        RequestSerialization();
    }

    public void TrueImoSubPickupState() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(TrueImoSubPickupStateSub));
    public void TrueImoSubPickupStateSub()
    {
        ImoSubPickupState = true;
        RequestSerialization();
    }

    public void FalseImoSubPickupState() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(FalseImoSubPickupStateSub));
    public void FalseImoSubPickupStateSub()
    {
        ImoSubPickupState = false;
        RequestSerialization();
    }

    public void CheckReset()
    {
        if (_imoSub1.ImoSubDisplayState == 0 && _imoSub2.ImoSubDisplayState == 0)
        {
            Reset();
        }
    }

    #endregion

    public void Reset()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            if (!Networking.LocalPlayer.IsOwner(_woodenStickObj.gameObject)) 
                Networking.SetOwner(Networking.LocalPlayer, _woodenStickObj.gameObject);
            if (!Networking.LocalPlayer.IsOwner(_imoMainPickupObj.gameObject))
                Networking.SetOwner(Networking.LocalPlayer, _imoMainPickupObj.gameObject);
            VRCPickup p0 = (VRCPickup)_woodenStickObj.GetComponent(typeof(VRCPickup));
            VRCPickup p1 = (VRCPickup)_imoMainPickupObj.GetComponent(typeof(VRCPickup));
            if (p0 != null) p0.Drop();
            if (p1 != null) p1.Drop();
            DropWoodenStickSub();
            _woodenStickObj.transform.localPosition = Vector3.zero;
            _woodenStickObj.transform.localRotation = Quaternion.identity;
            FalseWoodenStick();
            ImoMainDisplayState = false;
            FalseImoMainPickupState();
            FalseImoSubPickupState();
            FalseImoSubDisplay();
            RequestSerialization();
        }
    }
}
