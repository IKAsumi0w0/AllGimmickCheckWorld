
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class popper_01_blue : UdonSharpBehaviour
{
    public ParticleSystem ps;
    void Start()
    {
        
    }

    private void OnPickupUseDown()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, this.gameObject)) Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ParticlePlay");
    }

    public void ParticlePlay()
    {
        ps.Play();
    }
}
