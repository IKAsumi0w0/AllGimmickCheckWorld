
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IKA_DebugStartupHide : UdonSharpBehaviour
{
    [SerializeField] GameObject _obj;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(State))] bool _state = false;

    public bool State
    {
        get => _state;
        set
        {
            _state = value;
            if (_obj) _obj.SetActive(value);
        }
    }

    void Start()
    {
        State = false;
        RequestSerialization();
    }

    public override void Interact()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        State = !State;
        RequestSerialization();
    }
}
