
using UdonSharp;
using UnityEngine;
using UnityEngine.Animations;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
public class Greenonioncondimentspoon : UdonSharpBehaviour
{
    [SerializeField] ParentConstraint _parentConstraint;
    [SerializeField] Transform _parentTrans;
    [SerializeField] VRCPickup _pickup;
    [SerializeField] MeshRenderer _mr;
    float _resetTimer = 0f;
    float _resetDelay = 2f;
    int _resetCount = 0;


    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(NegiMeshRFlg))] bool _meshRFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(PickupState))] bool _pickupFlg = false;

    public bool NegiMeshRFlg
    {
        get => _meshRFlg;
        set
        {
            _meshRFlg = value;
            _mr.enabled = _meshRFlg;
        }
    }

    public bool PickupState
    {
        get => _pickupFlg;
        set
        {
            _pickupFlg = value;
            if (value)
            {
                _parentConstraint.constraintActive = false;
                for (int i = _parentConstraint.sourceCount - 1; i >= 0; --i)
                    _parentConstraint.RemoveSource(i);
            }
            else
            {
                for (int i = _parentConstraint.sourceCount - 1; i >= 0; --i)
                    _parentConstraint.RemoveSource(i);

                ConstraintSource source = new ConstraintSource();
                source.sourceTransform = _parentTrans;
                source.weight = 1.0f;
                int index = _parentConstraint.AddSource(source);

                _parentConstraint.SetTranslationOffset(index, Vector3.zero);
                _parentConstraint.SetRotationOffset(index, Vector3.zero);
                _parentConstraint.transform.position = _parentTrans.position;
                _parentConstraint.transform.rotation = _parentTrans.rotation;
                _parentConstraint.constraintActive = true;
            }
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
            }
        }
    }

    void Update()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            if (0 < ResetCount)
            {
                _resetTimer += Time.deltaTime;
                if (_resetTimer >= _resetDelay)
                {
                    ResetCount = 0;
                    _resetTimer = 0f;
                }
            }
        }
    }

    public override void OnPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (!PickupState)
        {
            PickupState = true;
            RequestSerialization();
        }
        ResetCount = 0;
    }

    public override void OnPickupUseDown()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            ++ResetCount;
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            Condimentcontainerwithgreenonions go = coll.GetComponent<Condimentcontainerwithgreenonions>();
            if (go != null)
            {
                NegiMeshRFlg = true;
                RequestSerialization();
            }
        }
    }

    public void HideNegi()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(HideNegiSub));
    }

    public void HideNegiSub()
    {
        NegiMeshRFlg = false;
        RequestSerialization();
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            PickupState = PickupState;
            NegiMeshRFlg = NegiMeshRFlg;
            RequestSerialization();
        }
    }

    public void Reset()
    {
        _pickup.Drop();
        PickupState = false;
        NegiMeshRFlg = false;
        ResetCount = 0;
        RequestSerialization();
    }
}
