
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ST_Spoon_PickupMain : UdonSharpBehaviour
{
    public ST_Spoon_PickupSub _sub;
    [SerializeField] MeshRenderer _meshR;
    [SerializeField] Collider _mainColl;
    [SerializeField] Collider _subColl;
    [SerializeField] MeshRenderer _sakuraMeshR;
    [SerializeField] AudioSource _hitSE, _eatSE;
    [SerializeField] protected ParticleSystem _eatPS;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(DisplayFlg))] bool _displayFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(SakuraFlg))] bool _sakuraFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ResetCount))] int _resetCount = 0;

    public bool DisplayFlg
    {
        get => _displayFlg;
        set
        {
            _displayFlg = value;
            _meshR.enabled = _displayFlg;
            _mainColl.enabled = _displayFlg;
            _subColl.enabled = _displayFlg;
        }
    }

    public bool SakuraFlg
    {
        get => _sakuraFlg;
        set
        {
            _sakuraFlg = value;
            _sakuraMeshR.enabled = _sakuraFlg;
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

    public void MainPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    public void MainDrop()
    {
        
    }

    public void MainPickupUseDown()
    {
        if (SakuraFlg)
        {
            SakuraFlg = false;
            PlayEatSE();
            PlayEatPS();
            RequestSerialization();
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            SakuraParfait_PickupMain sppm = coll.GetComponent<SakuraParfait_PickupMain>();
            if (sppm != null && !SakuraFlg)
            {
                SakuraFlg = true;
                PlayHitSE();
                RequestSerialization();
            }
        }

    }

    public void FuncDisplayFlg_ON() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(FuncDisplayFlg_ONSub));
    public void FuncDisplayFlg_ONSub()
    {
        DisplayFlg = true;
        RequestSerialization();
    }

    public void FuncDisplayFlg_OFF() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(FuncDisplayFlg_OFFSub));
    public void FuncDisplayFlg_OFFSub()
    {
        DisplayFlg = false;
        RequestSerialization();
    }

    public void PlayHitSE() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlayHitSESub));
    public void PlayHitSESub() => _hitSE.Play();

    public void PlayEatSE() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlayEatSESub));
    public void PlayEatSESub() => _eatSE.Play();

    public virtual void PlayEatPS() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlayEatPSSub));
    public virtual void PlayEatPSSub()
    {
        if (_eatPS)
        {
            _eatPS.Stop();
            _eatPS.Play();
        }
    }

    public void Reset()
    {
        VRCPickup p = _sub.GetComponent<VRCPickup>();
        if (p != null && p.IsHeld) p.Drop();
        SendCustomEventDelayedSeconds(nameof(SubReset), 0.5f, VRC.Udon.Common.Enums.EventTiming.Update);
        if (DisplayFlg)
        {
            DisplayFlg = false;
            RequestSerialization();
        }
        if (SakuraFlg)
        {
            SakuraFlg = false;
            RequestSerialization();
        }
    }

    public void SubReset()
    {
        _sub.gameObject.transform.localPosition = Vector3.zero;
        _sub.gameObject.transform.localRotation = Quaternion.identity;
    }
}
