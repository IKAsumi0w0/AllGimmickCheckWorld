
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class IKA_Sailor_House_AnimeOnOffSwitch : UdonSharpBehaviour
{
    public Animator animator;
    [SerializeField] bool _startFlg = false;
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

    void Start()
    {
        ToggleAnimeSwitch = _startFlg;
    }

    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(SwitchAnime));
    }

    public void SwitchAnime()
    {
        if (ToggleAnimeSwitch) ToggleAnimeSwitch = false;
        else ToggleAnimeSwitch = true;
    }
}
