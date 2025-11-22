
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ChocolateFountainWhite : UdonSharpBehaviour
{
    void OnTriggerEnter(Collider coll)
    {
        RealCharger_PickupMain rcpm = coll.GetComponent<RealCharger_PickupMain>();
        if (rcpm != null)
        {
            rcpm.AddWhiteFlg = true;
            RequestSerialization();
        }

        HeartFork_PickupMain hfpm = coll.GetComponent<HeartFork_PickupMain>();
        if (hfpm != null)
        {
            hfpm.AddWhiteFlg = true;
            RequestSerialization();
        }

        HeartLongFork_PickupMain hlfpm = coll.GetComponent<HeartLongFork_PickupMain>();
        if (hlfpm != null)
        {
            hlfpm.AddWhiteFlg = true;
            RequestSerialization();
        }
    }

    void OnTriggerExit(Collider coll)
    {
        RealCharger_PickupMain rcpm = coll.GetComponent<RealCharger_PickupMain>();
        if (rcpm != null)
        {
            rcpm.AddWhiteFlg = false;
            RequestSerialization();
        }

        HeartFork_PickupMain hfpm = coll.GetComponent<HeartFork_PickupMain>();
        if (hfpm != null)
        {
            hfpm.AddWhiteFlg = false;
            RequestSerialization();
        }

        HeartLongFork_PickupMain hlfpm = coll.GetComponent<HeartLongFork_PickupMain>();
        if (hlfpm != null)
        {
            hlfpm.AddWhiteFlg = false;
            RequestSerialization();
        }
    }
}
