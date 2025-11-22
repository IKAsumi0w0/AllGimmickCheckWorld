
using System.Security.Cryptography.X509Certificates;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Serialization.OdinSerializer;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class CakeNumCandleGimmick : CakeCandleBase
{
    [SerializeField] MeshRenderer[] _meshR;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CandleNo))] int _candleNo = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CandlePos))] Vector3 _candlePos = Vector3.zero;

    public int CandleNo { get => _candleNo; set => _candleNo = value; }

    protected override void OnDisplayFlgChanged()
    {
        if (DisplayFlg)
        {
            _meshR[CandleNo].enabled = true;
        }
        else
        {
            foreach (var mesh in _meshR)
            {
                mesh.enabled = false;
            }
        }
    }

    public Vector3 CandlePos
    {
        get => _candlePos;
        set
        {
            _candlePos = value;
            transform.localPosition = _candlePos;
        }
    }

    public void HideCandle()
    {
        CandlePos = Vector3.zero;
        DisplayFlg = false;
        FireFlg = false;
    }
}
