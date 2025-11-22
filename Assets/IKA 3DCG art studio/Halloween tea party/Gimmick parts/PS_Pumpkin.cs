
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PS_Pumpkin : UdonSharpBehaviour
{
    void OnParticleCollision(GameObject obj)
    {
        HalloweenTeaParty_Coll htpc = obj.GetComponent<HalloweenTeaParty_Coll>();
        if (htpc != null)
        {
            if (Networking.LocalPlayer.IsOwner(htpc.gameObject))
            {
                htpc._htpg.ShapekeyFloat -= 1.5f;
                RequestSerialization();
            }
        }
    }
}
