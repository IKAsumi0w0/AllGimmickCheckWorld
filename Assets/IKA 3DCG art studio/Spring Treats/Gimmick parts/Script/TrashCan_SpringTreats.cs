
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class TrashCan_SpringTreats : UdonSharpBehaviour
{
    void OnTriggerEnter(Collider coll)
    {
        SakuraParfait_PickupMain sppm = coll.GetComponent<SakuraParfait_PickupMain>();
        if (sppm != null)
        {
            sppm.Reset();
        }

        ST_Spoon_PickupMain stspm = coll.GetComponent<ST_Spoon_PickupMain>();
        if (stspm != null)
        {
            stspm.Reset();
        }

        IKA3D_SpwnGimmickBase_PickupMain ika3dsgbpm = coll.GetComponent<IKA3D_SpwnGimmickBase_PickupMain>();
        if (ika3dsgbpm != null)
        {
            ika3dsgbpm.Reset();
        }

        IKA3D_SpwnGimmick1_PickupMain ika3dsg1pm = coll.GetComponent<IKA3D_SpwnGimmick1_PickupMain>();
        if (ika3dsg1pm != null)
        {
            ika3dsg1pm.Reset();
        }

        IKA3D_SpwnGimmick2_PickupMain ika3dsg2pm = coll.GetComponent<IKA3D_SpwnGimmick2_PickupMain>();
        if (ika3dsg2pm != null)
        {
            ika3dsg2pm.Reset();
        }

        IKA3D_SpwnGimmick3_PickupMain ika3dsg3pm = coll.GetComponent<IKA3D_SpwnGimmick3_PickupMain>();
        if (ika3dsg3pm != null)
        {
            ika3dsg3pm.Reset();
        }

    }
}
