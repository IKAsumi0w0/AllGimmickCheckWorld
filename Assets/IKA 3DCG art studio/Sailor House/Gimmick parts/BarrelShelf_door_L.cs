
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BarrelShelf_door_L : UdonSharpBehaviour
{
    public Animator animator;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ToggleAnimeSwitch))] bool _flg = false;

    public bool ToggleAnimeSwitch
    {
        get => _flg;
        set
        {
            _flg = value;
            animator.SetBool("switchL", _flg);
        }
    }

    public override void Interact()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        ToggleAnimeSwitch = !ToggleAnimeSwitch;
        RequestSerialization();
    }
}
