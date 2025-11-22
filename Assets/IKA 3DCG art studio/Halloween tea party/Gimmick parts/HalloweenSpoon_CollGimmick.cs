
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class HalloweenSpoon_CollGimmick : UdonSharpBehaviour
{
    [SerializeField] HalloweenSpoon_Pickup _hsp;

    void OnTriggerEnter(Collider coll)
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            HalloweenPudding hp = coll.GetComponent<HalloweenPudding>();
            if (hp != null)
            {
                if (!_hsp.ShowFlg)
                {
                    _hsp.ShowFlg = true;
                    RequestSerialization();
                }
            }
        }


    }

}
