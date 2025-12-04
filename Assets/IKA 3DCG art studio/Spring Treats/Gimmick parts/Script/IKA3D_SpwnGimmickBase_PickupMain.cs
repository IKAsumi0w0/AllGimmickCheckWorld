
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IKA3D_SpwnGimmickBase_PickupMain : UdonSharpBehaviour
{
    public IKA3D_SpwnGimmickBase_PickupSub _sub;
    [SerializeField] protected MeshRenderer _meshR;
    [SerializeField] protected Collider _mainColl;
    [SerializeField] protected Collider _subColl;
    [SerializeField] protected AudioSource _eatSE;
    [SerializeField] protected ParticleSystem _eatPS;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(DisplayFlg))] protected bool _displayFlg = false;

    public virtual bool DisplayFlg
    {
        get => _displayFlg;
        set
        {
            _displayFlg = value;
            if (_meshR) _meshR.enabled = _displayFlg;
            if (_mainColl) _mainColl.enabled = _displayFlg;
            if (_subColl) _subColl.enabled = _displayFlg;
            OnDisplayFlgChanged();
        }
    }

    protected virtual void OnDisplayFlgChanged() { }

    protected virtual void Start()
    {

    }

    public virtual void MainPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    public virtual void MainPickupUseDown()
    {
        PlayEatSE();
        PlayEatPS();
        Reset();
    }

    public virtual void FuncDisplayFlg_ON() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(FuncDisplayFlg_ONSub));

    public virtual void FuncDisplayFlg_ONSub()
    {
        DisplayFlg = true;
        RequestSerialization();
    }

    public virtual void FuncDisplayFlg_OFF() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(FuncDisplayFlg_OFFSub));

    public virtual void FuncDisplayFlg_OFFSub()
    {
        DisplayFlg = false;
        RequestSerialization();
    }

    public virtual void PlayEatSE() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlayEatSESub));
    public virtual void PlayEatSESub() => _eatSE.Play();

    public virtual void PlayEatPS() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlayEatPSSub));
    public virtual void PlayEatPSSub()
    {
        if (_eatPS)
        {
            if (_eatPS.isPlaying)
            {
                _eatPS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            _eatPS.Play();
        }
    }

    public virtual void Reset()
    {
        VRCPickup p = _sub.GetComponent<VRCPickup>();
        if (p != null && p.IsHeld) p.Drop();
        SendCustomEventDelayedSeconds(nameof(SubReset), 0.5f, VRC.Udon.Common.Enums.EventTiming.Update);
        if (DisplayFlg)
        {
            DisplayFlg = false;
            RequestSerialization();
        }
    }

    public void SubReset()
    {
        _sub.gameObject.transform.localPosition = Vector3.zero;
        _sub.gameObject.transform.localRotation = Quaternion.identity;
    }
}
