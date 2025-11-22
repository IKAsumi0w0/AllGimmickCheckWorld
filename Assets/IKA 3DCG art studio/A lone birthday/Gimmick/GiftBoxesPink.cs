
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GiftBoxesPink : UdonSharpBehaviour
{
    [UdonSynced(UdonSyncMode.None)] private bool _switch = false;
    public Animator _anime;
    void Start()
    {
        
    }

    public override void Interact()
    {
        var player = Networking.LocalPlayer;

        if (player.IsOwner(this.gameObject))
        {
            Switching();
        }
        else
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(Switching));
        }
    }

    public override void OnDeserialization()
    {
        _anime.SetBool("switch", _switch);
    }

    public void Switching()
    {
        if (_switch) _switch = false;
        else _switch = true;
        RequestSerialization();
        _anime.SetBool("switch", _switch);
    }
}
