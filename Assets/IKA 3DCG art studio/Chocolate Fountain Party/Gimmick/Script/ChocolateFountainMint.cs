
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ChocolateFountainMint : UdonSharpBehaviour
{
    void OnTriggerEnter(Collider coll)
    {
        RealCharger_PickupMain rcpm = coll.GetComponent<RealCharger_PickupMain>();
        if (rcpm != null)
        {
            rcpm.AddMintFlg = true;
            RequestSerialization();
        }

        HeartFork_PickupMain hfpm = coll.GetComponent<HeartFork_PickupMain>();
        if (hfpm != null)
        {
            hfpm.AddMintFlg = true;
            RequestSerialization();
        }

        HeartLongFork_PickupMain hlfpm = coll.GetComponent<HeartLongFork_PickupMain>();
        if (hlfpm != null)
        {
            hlfpm.AddMintFlg = true;
            RequestSerialization();
        }
    }

    void OnTriggerExit(Collider coll)
    {
        RealCharger_PickupMain rcpm = coll.GetComponent<RealCharger_PickupMain>();
        if (rcpm != null)
        {
            rcpm.AddMintFlg = false;
            RequestSerialization();
        }

        HeartFork_PickupMain hfpm = coll.GetComponent<HeartFork_PickupMain>();
        if (hfpm != null)
        {
            hfpm.AddMintFlg = false;
            RequestSerialization();
        }

        HeartLongFork_PickupMain hlfpm = coll.GetComponent<HeartLongFork_PickupMain>();
        if (hlfpm != null)
        {
            hlfpm.AddMintFlg = false;
            RequestSerialization();
        }
    }
}
