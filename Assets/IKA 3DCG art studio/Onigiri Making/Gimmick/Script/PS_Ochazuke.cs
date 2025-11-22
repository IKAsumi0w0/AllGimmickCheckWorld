
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PS_Ochazuke : UdonSharpBehaviour
{
    void OnParticleCollision(GameObject obj)
    {
        Ochazuke_Pickup ocha = obj.GetComponent<Ochazuke_Pickup>();
        if (ocha != null)
        {
            if (Networking.LocalPlayer.IsOwner(ocha._main.gameObject))
            {
                if (!ocha._main.TeaState && ocha._main.TypeNo != 0)
                {
                    ocha._main.TeaState = true;
                    ocha._main.RequestSerialization();
                }
            }
        }
        Teacup_Pickup tcg0 = obj.GetComponent<Teacup_Pickup>();
        if (tcg0 != null)
        {
            if (Networking.LocalPlayer.IsOwner(tcg0.gameObject))
            {
                tcg0._main.ShapekeyFloat += 1.5f;
                tcg0._main.RequestSerialization();
            }
        }
    }
}
