
using UdonSharp;
using UnityEngine;
using UnityEngine.Animations;
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
    public ParentConstraint _constraint;
    float _throwForce = 5f;
    float _randomSpread = 2f;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ThrowDirection))] Vector3 _throwDirection = Vector3.zero;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ThrowTorque))] Vector3 _throwTorque = Vector3.zero;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(DisplayState))] protected bool _displayState = false;

    public Vector3 ThrowDirection
    {
        get => _throwDirection;
        set
        {
            _throwDirection = value;
        }
    }

    public Vector3 ThrowTorque
    {
        get => _throwTorque;
        set
        {
            _throwTorque = value;
        }
    }

    public bool DisplayState
    {
        get => _displayState;
        set
        {
            _displayState = value;
            _rb.useGravity = value;
            _rb.isKinematic = !value;
            _meshR.enabled = value;
            _constraint.constraintActive = !value;
            if (value)
            {
                SendCustomEventDelayedSeconds(nameof(DelayCollON), 0.1f, VRC.Udon.Common.Enums.EventTiming.Update);
            }
            else
            {
                _coll.enabled = false;
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
        }
    }

    public void DelayCollON() => _coll.enabled = true;

    public void SetThrowDirectionAndTorque()
    {

        ThrowDirection = Vector3.up + new Vector3(
            Random.Range(-_randomSpread, _randomSpread) * 0.1f,
            0,
            Random.Range(-_randomSpread, _randomSpread) * 0.1f
        );
        ThrowTorque = Random.insideUnitSphere;
        RequestSerialization();
    }

    public void ShootingBurntChip()
    {
        _rb.AddForce(ThrowDirection.normalized * _throwForce, ForceMode.Impulse);
        _rb.AddTorque(ThrowTorque * _throwForce, ForceMode.Impulse);
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
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(ResetSub));
    }

    public void ResetSub()
    {
        DisplayState = false;
        RequestSerialization();
    }
}
