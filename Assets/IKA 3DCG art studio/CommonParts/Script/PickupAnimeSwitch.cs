
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PickupAnimeSwitch : UdonSharpBehaviour
{
    public Animator animator;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ToggleAnimeSwitch))] private bool _flg = false;

    public bool ToggleAnimeSwitch
    {
        get => _flg;
        set
        {
            _flg = value;
            animator.SetBool("switch", _flg);
        }
    }

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    public override void OnPickupUseDown()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            ToggleAnimeSwitch = !ToggleAnimeSwitch;
        }
    }
}
