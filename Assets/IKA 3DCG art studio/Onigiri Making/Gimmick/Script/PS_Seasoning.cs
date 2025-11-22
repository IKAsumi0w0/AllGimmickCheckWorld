
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
public class PS_Seasoning : UdonSharpBehaviour
{
    void OnParticleCollision(GameObject obj)
    {
        Ochazuke_Pickup ocha = obj.GetComponent<Ochazuke_Pickup>();
        if (ocha != null && Networking.LocalPlayer.IsOwner(ocha._main.gameObject))
        {
            if (ocha._main.TeaState && !ocha._main.FrikakeState && ocha._main.TypeNo != 0)
            {
                ocha._main.FrikakeState = true;
                ocha._main.RequestSerialization();
            }
        }
    }
}
