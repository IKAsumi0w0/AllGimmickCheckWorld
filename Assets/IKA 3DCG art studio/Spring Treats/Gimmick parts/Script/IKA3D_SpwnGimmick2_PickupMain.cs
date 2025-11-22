
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class IKA3D_SpwnGimmick2_PickupMain : IKA3D_SpwnGimmickBase_PickupMain
{
    protected override void Start()
    {
        DisplayFlg = true;
    }

    public override void Reset()
    {
        base.Reset();
        SendCustomEventDelayedSeconds(nameof(DelayDisplayFlgON), 5f, VRC.Udon.Common.Enums.EventTiming.Update);
    }

    public void DelayDisplayFlgON()
    {
        DisplayFlg = true;
        RequestSerialization();
    }

}
