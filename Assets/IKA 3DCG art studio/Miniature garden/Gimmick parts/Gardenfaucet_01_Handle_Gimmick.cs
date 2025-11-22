
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

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

    void Start()
    {

    }

    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(ShowSwitch));
    }

    public void ShowSwitch()
    {
        if (AnimeSwitch)
        {
            AnimeSwitch = false;
        }
        else
        {
            AnimeSwitch = true;
        }
    }
}
