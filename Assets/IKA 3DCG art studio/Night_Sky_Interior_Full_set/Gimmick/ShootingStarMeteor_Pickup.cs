
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class ShootingStarMeteor_Pickup : UdonSharpBehaviour
{
    public Rigidbody _pickupRigi;
    public GameObject _pickupObj;
    public Animator _anime;
    public GameObject _meteorSetObj;
    public GameObject _meteorPrefab;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(PickupFlg))] private bool _pickupFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(LifeTimer))] private float _lifeTimer = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(LifeCount))] private float _lifeCount = 0;
    private Bom_ShootingStar _bss;

    public bool PickupFlg
    {
        get => _pickupFlg;
        set
        {
            _pickupFlg = value;
            if (_pickupFlg) _anime.SetTrigger("trig");
            _pickupRigi.isKinematic = _pickupFlg;
        }
    }

    public float LifeTimer
    {
        get => _lifeTimer;
        set
        {
            _lifeTimer = value;
        }
    }

    public float LifeCount
    {
        get => _lifeCount;
        set
        {
            _lifeCount = value;
        }
    }

    void Start()
    {
        _bss = this.gameObject.transform.parent.parent.parent.GetComponent<Bom_ShootingStar>();
        _pickupRigi.isKinematic = false;
    }

    void Update()
    {
        if (!PickupFlg)
        {
            LifeCount += Time.deltaTime;
        }
    }

    void OnEnable()
    {
        SendCustomEventDelayedSeconds(nameof(ReturnStar), 60f, VRC.Udon.Common.Enums.EventTiming.Update);
        SendCustomEventDelayedSeconds(nameof(ReturnStar), 65f, VRC.Udon.Common.Enums.EventTiming.Update);
    }

    public override void OnPickup()
    {
        SendCustomEventDelayedSeconds(nameof(StarPickup), 2f, VRC.Udon.Common.Enums.EventTiming.Update);
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            StarPickupOwner();
        }
        else
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(StarPickupOwner));
        }
    }

    public override void OnDrop()
    {
        
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            StarDrop();
        }
        else
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(StarDrop));
        }
    }

    public void StarPickup()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(StarPickupAll));
    }

    public void StarPickupAll()
    {
        GameObject meteor = VRCInstantiate(_meteorPrefab);
        meteor.transform.localPosition = Vector3.zero;
        meteor.transform.localRotation = Quaternion.identity;
        meteor.transform.position = _bss._shootPos.transform.position;
        meteor.transform.forward = (this.transform.position - _bss._shootPos.transform.position).normalized;
        meteor.GetComponent<Rigidbody>().velocity = meteor.transform.forward * 20f;
        _pickupRigi.velocity = Vector3.zero;
        _pickupRigi.angularVelocity = Vector3.zero;
    }

    public void StarPickupOwner()
    {
        PickupFlg = true;
    }

    public void StarDrop()
    {
        PickupFlg = false;
    }

    public void ReturnStar()
    {
        PickupFlg = false;
        LifeTimer = 10f;
        LifeCount += 9999f;
    }
}
