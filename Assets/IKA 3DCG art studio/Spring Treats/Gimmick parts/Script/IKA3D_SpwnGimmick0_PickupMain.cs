
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IKA3D_SpwnGimmick0_PickupMain : IKA3D_SpwnGimmickBase_PickupMain
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

    public override void FuncDisplayFlg_ONSub()
    {
        base.FuncDisplayFlg_ONSub(); // 🟢 親クラスの処理を適用
        MeshNo = Random.Range(0, _subMeshR.Length);
        DisplayFlg = true;
        RequestSerialization();
    }
}
