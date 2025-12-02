
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Dial_02 : UdonSharpBehaviour
{
    [SerializeField] GameObject _dropObj;
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
                    PoronReset();
                }
            }
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            Dial_poron dial_Poron = coll.GetComponent<Dial_poron>();
            if (dial_Poron != null)
            {
                _count = 0;
                VRC_Pickup pickup = (VRC_Pickup)dial_Poron.gameObject.GetComponent(typeof(VRC_Pickup));
                if (pickup != null)
                {
                    pickup.Drop();
                }
                Rigidbody rb = dial_Poron.gameObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    dial_Poron.gameObject.transform.localPosition = Vector3.zero;
                    dial_Poron.gameObject.transform.localRotation = Quaternion.identity;
                }

                PoronReset();
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
            SendCustomEventDelayedSeconds(nameof(CollTrueSwitch), 2f, VRC.Udon.Common.Enums.EventTiming.Update);
            DropModelSwitch = true;
            Dial02ModelSwitch = false;
            Rigidbody rb = _dropObj.gameObject.GetComponent<Rigidbody>();
            Vector3 force = this.gameObject.transform.forward * 1f;
            rb.AddForce(force, ForceMode.Impulse);
            RequestSerialization();
        }
    }

    public void CollTrueSwitch()
    {
        CollModelSwitch = true;
        RequestSerialization();
    }

    public void PoronReset()
    {
        DropModelSwitch = false;
        Dial02ModelSwitch = true;
        RequestSerialization();
    }
}
