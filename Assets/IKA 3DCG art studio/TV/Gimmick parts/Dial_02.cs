
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Dial_02 : UdonSharpBehaviour
{
    [SerializeField] GameObject _dropObj;
    [SerializeField] Rigidbody _dropRB;
    [SerializeField] MeshRenderer _dialMR;
    [SerializeField] SphereCollider _coll;
    float _count = 0;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(DropModelSwitch))] bool _dropflg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CollModelSwitch))] bool _collFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(Dial02ModelSwitch))] bool _dial02Flg = false;

    public bool DropModelSwitch
    {
        get => _dropflg;
        set
        {
            _dropflg = value;
            _dropObj.SetActive(value);
        }
    }

    public bool CollModelSwitch
    {
        get => _collFlg;
        set
        {
            _collFlg = value;
            _coll.enabled = _collFlg;
        }
    }

    public bool Dial02ModelSwitch
    {
        get => _dial02Flg;
        set
        {
            _dial02Flg = value;
            _dialMR.enabled = value;
        }
    }

    void Update()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            if (DropModelSwitch && CollModelSwitch)
            {
                _count += Time.deltaTime;
                if (30f < _count)
                {
                    PoronReset1();
                }
            }
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            Dial_poron dial_Poron = coll.GetComponent<Dial_poron>();
            if (_dropObj == coll.gameObject)
            {
                _count = 0;
                VRC_Pickup pickup = (VRC_Pickup)_dropObj.gameObject.GetComponent(typeof(VRC_Pickup));
                if (pickup != null)
                {
                    pickup.Drop();
                }
                SendCustomNetworkEvent(NetworkEventTarget.All, nameof(PoronReset2));
                PoronReset1();
            }
        }
    }

    public override void Interact()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (!Networking.LocalPlayer.IsOwner(_dropObj.gameObject)) Networking.SetOwner(Networking.LocalPlayer, _dropObj.gameObject);
        DropSwitch();
    }

    public void DropSwitch()
    {
        if (!DropModelSwitch)
        {
            CollModelSwitch = false;
            SendCustomEventDelayedSeconds(nameof(DelayDropSwitch), 0.2f, VRC.Udon.Common.Enums.EventTiming.Update);
            SendCustomEventDelayedSeconds(nameof(CollTrueSwitch), 3f, VRC.Udon.Common.Enums.EventTiming.Update);
            RequestSerialization();
        }
    }

    public void DelayDropSwitch()
    {
        DropModelSwitch = true;
        Dial02ModelSwitch = false;
        RequestSerialization();
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(ShootDial));
    }

    public void ShootDial()
    {
        _dropRB.AddForce(gameObject.transform.forward, ForceMode.Impulse);
    }

    public void CollTrueSwitch()
    {
        CollModelSwitch = true;
        RequestSerialization();
    }

    public void PoronReset1()
    {
        DropModelSwitch = false;
        Dial02ModelSwitch = true;
        RequestSerialization();
    }

    public void PoronReset2()
    {
        _dropRB.velocity = Vector3.zero;
        _dropRB.angularVelocity = Vector3.zero;
        _dropRB.gameObject.transform.localPosition = Vector3.zero;
        _dropRB.gameObject.transform.localRotation = Quaternion.identity;
    }
}
