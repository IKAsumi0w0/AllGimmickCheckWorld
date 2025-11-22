
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Dial_02 : UdonSharpBehaviour
{
    [SerializeField] private GameObject _dropObj;
    [SerializeField] private GameObject _dial02Obj;
    [SerializeField] private SphereCollider _coll;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(DropModelSwitch))] private bool _dropflg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CollModelSwitch))] private bool _collFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(Dial02ModelSwitch))] private bool _dial02Flg = false;

    public bool DropModelSwitch
    {
        get => _dropflg;
        set
        {
            _dropflg = value;
            _dropObj.SetActive(_dropflg);
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
            _dial02Obj.SetActive(_dial02Flg);
        }
    }

    public override void Interact()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            DropSwitch();
        }
        else
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(DropSwitch));
        }
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
        }
    }

    public void CollTrueSwitch()
    {
        CollModelSwitch = true;
    }


}
