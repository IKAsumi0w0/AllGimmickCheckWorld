
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class SBTSet_SmashedbeanToast : UdonSharpBehaviour
{
    [SerializeField] AudioSource _as;

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    public override void OnPickupUseDown()
    {
        SendCustomEventDelayedSeconds(nameof(Respawn), 0.5f, VRC.Udon.Common.Enums.EventTiming.Update);
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlaySe));
        VRCPickup obj = (VRCPickup)gameObject.GetComponent(typeof(VRCPickup));
        if (obj != null)
        {
            obj.Drop();
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    public void Respawn()
    {
        VRCPickup obj = (VRCPickup)gameObject.GetComponent(typeof(VRCPickup));
        if (obj != null)
        {
            obj.Drop();
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }

    public void PlaySe()
    {
        _as.Play();
    }
}
