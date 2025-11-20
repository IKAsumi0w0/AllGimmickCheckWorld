
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class Meteor : UdonSharpBehaviour
{
    public GameObject _meteoObj;
    public GameObject _meteoObjEx;
    private Rigidbody _rigi;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(MeteorHitFlg))] public bool _meteorHitFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(MeteorShowFlg))] public bool _meteorShowFlg = true;

    public bool MeteorHitFlg
    {
        get => _meteorHitFlg;
        set
        {
            _meteorHitFlg = value;
        }
    }

    public bool MeteorShowFlg
    {
        get => _meteorShowFlg;
        set
        {
            _meteorShowFlg = value;
            _meteoObj.SetActive(_meteorShowFlg);
            _meteoObjEx.SetActive(!_meteorShowFlg);
        }
    }

    void Start()
    {
        _rigi = this.GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        SendCustomEventDelayedSeconds(nameof(DestroyObj), 15f, VRC.Udon.Common.Enums.EventTiming.Update);
    }

    void OnTriggerEnter(Collider other)
    {
        //_explosionObj.transform.position = this.gameObject.transform.position;
        MeteorShowFlg = false;
        _rigi.velocity = Vector3.zero;
        _rigi.angularVelocity = Vector3.zero;
        SendCustomEventDelayedSeconds(nameof(ReSpawnMeteor), 10f, VRC.Udon.Common.Enums.EventTiming.Update);
    }

    public void ReSpawnMeteor()
    {
        if (Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            ReSpawnMeteorOwner();
        }
        else
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(ReSpawnMeteorOwner));
        }
    }

    public void ReSpawnMeteorOwner()
    {
        MeteorHitFlg = true;
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(DestroyObj));
    }

    public void DestroyObj()
    {
        Destroy(this.gameObject);
    }
}
