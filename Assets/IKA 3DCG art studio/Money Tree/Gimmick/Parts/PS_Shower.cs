
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PS_Shower : UdonSharpBehaviour
{
    void OnParticleCollision(GameObject obj)
    {
        MoneyTreeColl mtc = obj.GetComponent<MoneyTreeColl>();
        if (mtc != null)
        {
            if (Networking.LocalPlayer.IsOwner(mtc._main.gameObject))
            {
                mtc._main.AnimeFloat += 1f;
                RequestSerialization();
            }
        }
    }
}
