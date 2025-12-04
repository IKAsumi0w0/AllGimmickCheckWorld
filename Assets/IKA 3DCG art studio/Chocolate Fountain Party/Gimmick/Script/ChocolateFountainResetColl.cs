
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ChocolateFountainResetColl : UdonSharpBehaviour
{
    void OnEnable()
    {
        SendCustomEventDelayedSeconds(nameof(HideClearColl), 0.5f, VRC.Udon.Common.Enums.EventTiming.Update);
    }

    public void HideClearColl()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerStay(Collider coll)
    {
        ChocolateFountain_PickupMain cfpm = coll.GetComponent<ChocolateFountain_PickupMain>();
        if (cfpm != null && Networking.LocalPlayer.IsOwner(cfpm.gameObject))
        {
            cfpm.Reset();
            RequestSerialization();
        }

        RealCharger_PickupMain rcpm = coll.GetComponent<RealCharger_PickupMain>();
        if (rcpm != null && Networking.LocalPlayer.IsOwner(rcpm.gameObject))
        {
            rcpm.Reset();
            RequestSerialization();
        }

        HeartFork_PickupMain hfpm = coll.GetComponent<HeartFork_PickupMain>();
        if (hfpm != null && Networking.LocalPlayer.IsOwner(hfpm.gameObject))
        {
            hfpm.Reset();
            RequestSerialization();
        }

        HeartLongFork_PickupMain hlfpm = coll.GetComponent<HeartLongFork_PickupMain>();
        if (hlfpm != null && Networking.LocalPlayer.IsOwner(hlfpm.gameObject))
        {
            hlfpm.Reset();
            RequestSerialization();
        }
    }
}
