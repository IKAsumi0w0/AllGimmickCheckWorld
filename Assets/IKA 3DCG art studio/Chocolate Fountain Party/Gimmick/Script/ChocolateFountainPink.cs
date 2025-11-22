
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ChocolateFountainPink : UdonSharpBehaviour
{
    void OnTriggerEnter(Collider coll)
    {
        RealCharger_PickupMain rcpm = coll.GetComponent<RealCharger_PickupMain>();
        if (rcpm != null)
        {
            rcpm.AddPinkFlg = true;
            RequestSerialization();
        }

        HeartFork_PickupMain hfpm = coll.GetComponent<HeartFork_PickupMain>();
        if (hfpm != null)
        {
            hfpm.AddPinkFlg = true;
            RequestSerialization();
        }

        HeartLongFork_PickupMain hlfpm = coll.GetComponent<HeartLongFork_PickupMain>();
        if (hlfpm != null)
        {
            hlfpm.AddPinkFlg = true;
            RequestSerialization();
        }
    }

    void OnTriggerExit(Collider coll)
    {
        RealCharger_PickupMain rcpm = coll.GetComponent<RealCharger_PickupMain>();
        if (rcpm != null)
        {
            rcpm.AddPinkFlg = false;
            RequestSerialization();
        }

        HeartFork_PickupMain hfpm = coll.GetComponent<HeartFork_PickupMain>();
        if (hfpm != null)
        {
            hfpm.AddPinkFlg = false;
            RequestSerialization();
        }

        HeartLongFork_PickupMain hlfpm = coll.GetComponent<HeartLongFork_PickupMain>();
        if (hlfpm != null)
        {
            hlfpm.AddPinkFlg = false;
            RequestSerialization();
        }
    }
}
