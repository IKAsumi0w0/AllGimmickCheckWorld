
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Gardenfaucet_01_Handle_Gimmick : UdonSharpBehaviour
{
    [SerializeField] private Animator _anime;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(AnimeSwitch))] private bool _flg = false;

    public bool AnimeSwitch
    {
        get => _flg;
        set
        {
            _flg = value;
            _anime.SetBool("switch", _flg);
        }
    }

    public override void Interact()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        AnimeSwitch = !AnimeSwitch;
        RequestSerialization();
    }
}
