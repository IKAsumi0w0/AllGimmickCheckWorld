
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IKA_FireworksTorch : UdonSharpBehaviour
{
    void OnTriggerStay(Collider other)
    {
        IKA_FireworksIgnition ikaFwI = other.GetComponent<IKA_FireworksIgnition>();
        if (ikaFwI != null && !ikaFwI.TogglePsObj)
        {
            if (Networking.LocalPlayer.IsOwner(gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, ikaFwI.gameObject);
                ikaFwI.TogglePsObj = true;
                ikaFwI.RequestSerialization();
            }
        }
        IKA_HandFireworksSusuki ikaFwIS = other.GetComponent<IKA_HandFireworksSusuki>();
        if (ikaFwIS != null && !ikaFwIS._main.TogglePsObj)
        {
            if (Networking.LocalPlayer.IsOwner(ikaFwIS._main.gameObject))
            {
                ikaFwIS._main.TogglePsObj = true;
                ikaFwIS._main.RequestSerialization();
            }
        }
    }
}
