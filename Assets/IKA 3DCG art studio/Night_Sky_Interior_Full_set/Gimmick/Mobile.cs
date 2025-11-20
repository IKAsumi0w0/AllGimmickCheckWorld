
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Mobile : UdonSharpBehaviour
{
    [SerializeField] Animator _anim;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(AnimeSwitch))] private float _animeValue = 0;

    public float AnimeSwitch
    {
        get => _animeValue;
        set
        {
            _animeValue = value;
            _anim.SetFloat("val", AnimeSwitch);
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

    private void SetAnimePara()
    {
        if (0 < AnimeSwitch)
        {
            AnimeSwitch = 0f;
        }
        else
        {
            AnimeSwitch = 1f;
        }
    }

}
