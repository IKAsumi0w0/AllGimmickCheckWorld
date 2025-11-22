
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IKA3D_SpwnGimmick0_Manager : IKA3D_SpwnGimmickBase_Manager
{
    protected override void Start()
    {
        ShowFlg = false;
    }

    public override void SpawnObj()
    {
        foreach (var obj in _objs)
        {
            if (obj._main.DisplayFlg && obj.transform.localPosition == Vector3.zero)
            {
                return;
            }
        }

        foreach (var obj in _objs)
        {
            if (!obj._main.DisplayFlg)
            {
                obj._main.FuncDisplayFlg_ON();
                RequestSerialization();
                return;
            }
        }
    }

}
