
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ChocolateFountainChoco : UdonSharpBehaviour
{
    void OnTriggerEnter(Collider coll)
    {
        RealCharger_PickupMain rcpm = coll.GetComponent<RealCharger_PickupMain>();
        if (rcpm != null)
        {
            rcpm.AddChocoFlg = true;
            RequestSerialization();
        }

        HeartFork_PickupMain hfpm = coll.GetComponent<HeartFork_PickupMain>();
        if (hfpm != null)
        {
            hfpm.AddChocoFlg = true;
            RequestSerialization();
        }

        HeartLongFork_PickupMain hlfpm = coll.GetComponent<HeartLongFork_PickupMain>();
        if (hlfpm != null)
        {
            hlfpm.AddChocoFlg = true;
            RequestSerialization();
        }
    }

    void OnTriggerExit(Collider coll)
    {
        RealCharger_PickupMain rcpm = coll.GetComponent<RealCharger_PickupMain>();
        if (rcpm != null)
        {
            rcpm.AddChocoFlg = false;
            RequestSerialization();
        }

        HeartFork_PickupMain hfpm = coll.GetComponent<HeartFork_PickupMain>();
        if (hfpm != null)
        {
            hfpm.AddChocoFlg = false;
            RequestSerialization();
        }

        HeartLongFork_PickupMain hlfpm = coll.GetComponent<HeartLongFork_PickupMain>();
        if (hlfpm != null)
        {
            hlfpm.AddChocoFlg = false;
            RequestSerialization();
        }
    }
}
