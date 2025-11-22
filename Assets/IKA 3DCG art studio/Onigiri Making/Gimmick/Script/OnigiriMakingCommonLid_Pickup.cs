
using UdonSharp;
using UnityEngine;
using UnityEngine.Animations;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class OnigiriMakingCommonLid_Pickup : UdonSharpBehaviour
{
    [SerializeField] ParentConstraint _parentConstraint;
    [SerializeField] Transform _parentTrans;
    [SerializeField] VRCPickup _pickup;
    float _resetTimer = 0f;
    float _resetDelay = 2f;
    int _resetCount = 0;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(PickupState))] bool _pickupFlg = false;

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
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        PickupState = true;
        ResetCount = 0;
    }

    public override void OnPickupUseDown()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            ++ResetCount;
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            PickupState = PickupState;
        }
    }

    public void Reset()
    {
        _pickup.Drop();
        PickupState = false;
        ResetCount = 0;
        RequestSerialization();
    }
}
