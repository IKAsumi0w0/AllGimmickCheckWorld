
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Fan_center : UdonSharpBehaviour
{
    [SerializeField] Animator _anime = default;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(AnimeFlg))] public bool _flg = true;

    public bool AnimeFlg
    {
        get => _flg;
        set
        {
            _flg = value;
            _anime.SetBool("flg", _flg);
        }
    }

    void Start()
    {
        
    }

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    public override void OnPickupUseDown()
    {
        AnimeFlg = !AnimeFlg;
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer == player)
        {
            _anime.SetBool("flg", AnimeFlg);
        }
    }
}
