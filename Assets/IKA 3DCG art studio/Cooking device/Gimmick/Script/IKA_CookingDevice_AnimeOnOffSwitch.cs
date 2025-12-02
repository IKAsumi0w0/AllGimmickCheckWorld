
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IKA_CookingDevice_AnimeOnOffSwitch : UdonSharpBehaviour
{
    public Animator animator;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ToggleAnimeSwitch))] public bool _flg = false;

    public bool ToggleAnimeSwitch
    {
        get => _flg;
        set
        {
            _flg = value;
            animator.SetBool("switch", _flg);
        }
    }

    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(SwitchAnime));
    }

    public void SwitchAnime()
    {
        ToggleAnimeSwitch = !ToggleAnimeSwitch;
        RequestSerialization();
    }
}
