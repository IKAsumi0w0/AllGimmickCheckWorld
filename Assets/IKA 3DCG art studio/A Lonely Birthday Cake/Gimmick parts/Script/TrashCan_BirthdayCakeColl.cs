
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class TrashCan_BirthdayCakeColl : UdonSharpBehaviour
{
    void OnTriggerEnter(Collider coll)
    {
        WholeCake_PickupMain wcpm = coll.GetComponent<WholeCake_PickupMain>();
        if (wcpm != null)
        {
            wcpm.Reset();
        }

        WholeCakeFork_PickupMain wcfpm = coll.GetComponent<WholeCakeFork_PickupMain>();
        if (wcfpm != null)
        {
            wcfpm.Reset();
        }

        EditCakeRod_PickupMain ecrpm = coll.GetComponent<EditCakeRod_PickupMain>();
        if (ecrpm != null)
        {
            ecrpm.Reset();
        }

        IgnitionRod_PickupMain irpm = coll.GetComponent<IgnitionRod_PickupMain>();
        if (irpm != null)
        {
            irpm.Reset();
        }
    }
}
