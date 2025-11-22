
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class TrashCan_BirthdayCake : UdonSharpBehaviour
{
    [SerializeField] Collider _coll;

    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ShowColl));
    }

    public void ShowColl()
    {
        _coll.enabled = true;
        SendCustomEventDelayedSeconds(nameof(HideColl), 0.1f);
    }

    public void HideColl()
    {
        _coll.enabled = false;
    }
}
