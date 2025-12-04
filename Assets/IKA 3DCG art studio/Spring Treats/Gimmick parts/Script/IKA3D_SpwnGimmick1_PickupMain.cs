
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IKA3D_SpwnGimmick1_PickupMain : IKA3D_SpwnGimmickBase_PickupMain
{
    [SerializeField] GameObject _meshRObj;
    [SerializeField] MeshRenderer[] _subMeshR;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(MeshNo))] int _cakeNoMain = 0;

    public int MeshNo
    {
        get => _cakeNoMain;
        set
        {
            _cakeNoMain = value;
            foreach (MeshRenderer item in _subMeshR)
            {
                item.enabled = false;
            }
            _subMeshR[_cakeNoMain].enabled = true;
        }
    }

    protected override void OnDisplayFlgChanged()
    {
        _meshRObj.SetActive(DisplayFlg);
    }

    public override void MainPickupUseDown()
    {
        PlayEatSE();
        PlayEatPS();
        if (MeshNo == 0) MeshNo = 1;
        else if (MeshNo == 1)
        {
            MeshNo = 0;
            Reset();
        }
        RequestSerialization();
    }

    public override void FuncDisplayFlg_ONSub()
    {
        base.FuncDisplayFlg_ONSub();
        if (MeshNo != 0)
        {
            MeshNo = 0;
            RequestSerialization();
        }
    }

    public override void Reset()
    {
        base.Reset();
        if (MeshNo != 0)
        {
            MeshNo = 0;
            RequestSerialization();
        }
    }
}
