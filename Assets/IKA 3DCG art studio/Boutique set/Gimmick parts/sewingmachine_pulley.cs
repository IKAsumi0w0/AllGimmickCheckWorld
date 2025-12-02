
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class sewingmachine_pulley : UdonSharpBehaviour
{
    public Animator _anime;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(AnimeFlg))] private bool _animeFlg = false;

    public bool AnimeFlg
    {
        get => _animeFlg;
        set
        {
            _animeFlg = value;
            _anime.SetBool("MachineSwitch", _animeFlg);
        }
    }

    public override void Interact()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        AnimeFlg = !AnimeFlg;
        RequestSerialization();
    }
}
