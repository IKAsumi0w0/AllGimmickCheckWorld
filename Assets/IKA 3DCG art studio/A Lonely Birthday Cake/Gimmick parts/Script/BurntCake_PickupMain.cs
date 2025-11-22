
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BurntCake_PickupMain : UdonSharpBehaviour
{
    public BurntCake_PickupSub _sub;
    public MeshRenderer _meshR;
    public Rigidbody _rb;
    public Collider _coll;
    public Transform _parent;
    public WholeCake_PickupMain _wcpm;
    float _throwForce = 5f;
    float _randomSpread = 2f;

    public void Show()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ShowSub));
    }

    public void ShowSub()
    {
        _rb.useGravity = true;
        _rb.isKinematic = false;
        SendCustomEventDelayedSeconds(nameof(DelayCollON), 0.1f, VRC.Udon.Common.Enums.EventTiming.Update);
        _meshR.enabled = true;
        _sub.transform.parent = null;
    }

    public void DelayCollON() => _coll.enabled = true;

    public void HideSub()
    {
        _rb.useGravity = false;
        _rb.isKinematic = true;
        _coll.enabled = false;
        _meshR.enabled = false;
        _sub.transform.parent = _parent;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _sub.gameObject.transform.localPosition = Vector3.zero;
        _sub.gameObject.transform.localRotation = Quaternion.identity;
    }

    public void SetFly()
    {
        Vector3 throwDirection = Vector3.up + new Vector3(
            Random.Range(-_randomSpread, _randomSpread) * 0.1f,
            0,
            Random.Range(-_randomSpread, _randomSpread) * 0.1f
        );
        _rb.AddForce(throwDirection.normalized * _throwForce, ForceMode.Impulse);
        _rb.AddTorque(Random.insideUnitSphere * _throwForce, ForceMode.Impulse);
    }

    public void MainPickup()
    {

    }

    public void MainDrop()
    {

    }

    public void MainPickupUseDown()
    {
        VRCPickup pickup = (VRCPickup)_sub.gameObject.GetComponent(typeof(VRCPickup));
        if (pickup != null)
        {
            pickup.Drop();
        }
        Reset();
    }

    public void Reset()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ResetSub));
    }

    public void ResetSub()
    {
        HideSub();
    }
}
