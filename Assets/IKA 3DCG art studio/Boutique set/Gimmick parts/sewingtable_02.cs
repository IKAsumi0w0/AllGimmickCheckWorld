
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

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
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            SetAnimePara();
        }
        else
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(SetAnimePara));
        }
    }

    public void SetAnimePara()
    {
        if (AnimeFlg)
        {
            AnimeFlg = false;
        }
        else
        {
            AnimeFlg = true;
        }
    }
}
