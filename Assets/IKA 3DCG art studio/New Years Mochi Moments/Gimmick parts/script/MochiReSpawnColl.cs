
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class MochiReSpawnColl : UdonSharpBehaviour
{
    void OnTriggerStay(Collider coll)
    {
        MochiSpawn mochiSpawn = coll.gameObject.GetComponent<MochiSpawn>();
        if (mochiSpawn != null)
        {
            mochiSpawn.AllReSpawnMochi();
        }
        MochiMain mochi = coll.gameObject.GetComponent<MochiMain>();
        if (mochi != null)
        {
            mochi.ResetFlg = true;
            RequestSerialization();
        }
    }
}
