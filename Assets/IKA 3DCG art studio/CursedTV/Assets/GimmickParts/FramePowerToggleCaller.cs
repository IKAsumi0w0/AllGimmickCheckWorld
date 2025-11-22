using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FramePowerToggleCaller : UdonSharpBehaviour
{
    [Header("Target Udon (FrameSwitchWithGlitchSeconds)")]
    public FrameSwitchWithGlitchSeconds _targetBehaviour;

    public override void Interact()
    {
        if (_targetBehaviour == null) return;
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        _targetBehaviour._TogglePowerNet();
    }

}
