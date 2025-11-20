
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class IKA_TwoMillion_Switch : UdonSharpBehaviour
{
    public Animator _anime;
    void Start()
    {
        
    }
    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayAnimation");
    }

    public void PlayAnimation()
    {
        _anime.SetTrigger("switch");
    }
}
