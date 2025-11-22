
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class CakeSpecialCandleGimmick : CakeCandleBase
{
    [SerializeField] MeshRenderer _meshR;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CandleType))] private int _candleType = 0;

    public int CandleType { get => _candleType; set => _candleType = value; }

    protected override void OnDisplayFlgChanged()
    {
        if (_meshR != null) _meshR.enabled = DisplayFlg;
    }

}
