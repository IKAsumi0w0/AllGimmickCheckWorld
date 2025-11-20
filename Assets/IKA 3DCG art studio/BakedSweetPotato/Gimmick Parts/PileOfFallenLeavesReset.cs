
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PileOfFallenLeavesReset : UdonSharpBehaviour
{
    [SerializeField] private GameObject _resetObj;

    void Start()
    {
        _resetObj.SetActive(false);
    }

    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(AllResetIKAHF));
    }

    public void AllResetIKAHF()
    {
        _resetObj.SetActive(true);
        SendCustomEventDelayedSeconds(nameof(ResetObjHide), 0.1f, VRC.Udon.Common.Enums.EventTiming.Update);
    }

    public void ResetObjHide() { _resetObj.SetActive(false); }
}
