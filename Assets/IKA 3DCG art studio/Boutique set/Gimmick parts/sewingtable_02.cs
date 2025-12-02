
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class sewingtable_02 : UdonSharpBehaviour
{
    public Animator _anime;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(AnimeFlg))] private bool _animeFlg = false;

    public bool AnimeFlg
    {
        get => _animeFlg;
        set
        {
            _animeFlg = value;
            _anime.SetBool("ShelfSwitch", _animeFlg);
        }
    }

    public override void Interact()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        AnimeFlg = !AnimeFlg;
        RequestSerialization();
    }
}
