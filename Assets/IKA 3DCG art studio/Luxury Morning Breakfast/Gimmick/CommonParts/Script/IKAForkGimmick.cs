
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class IKAForkGimmick : UdonSharpBehaviour
{
    [SerializeField] IKAForkTipGimmick _tip;
    [SerializeField] AudioSource _audioSource;


    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        Networking.SetOwner(Networking.LocalPlayer, _tip.gameObject);
    }

    public override void OnPickupUseDown()
    {
        if (_tip.SetFlg)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlaySE));
            SendCustomEventDelayedSeconds(nameof(Reset), 0.5f, VRC.Udon.Common.Enums.EventTiming.Update);
        }
    }


    public void PlaySE()
    {
        _audioSource.Play();
    }

    public void Reset()
    {
        _tip.SetFlg = false;
    }
}
