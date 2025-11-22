using UdonSharp;
using UnityEngine;
using VRC.Udon.Serialization.OdinSerializer;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class CakeCandleGimmick : CakeCandleBase
{
    [SerializeField] GameObject _mainCandleObj;
    [SerializeField] float _candleHeight0, _candleHeight1;
    [OdinSerialize] public MeshRenderer[][] _meshR;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CandleTypeMain))] int _candleTypeMain = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CandleTypeSub))] int _candleTypeSub = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CandlePos))] Vector3 _candlePos = Vector3.zero;

    public int CandleTypeMain { get => _candleTypeMain; set => _candleTypeMain = value; }
    public int CandleTypeSub { get => _candleTypeSub; set => _candleTypeSub = value; }

    protected override void OnDisplayFlgChanged()
    {
        if (_mainCandleObj != null) _mainCandleObj.SetActive(DisplayFlg);
        HideCandle();

        if (DisplayFlg)
        {
            _meshR[CandleTypeMain][CandleTypeSub].enabled = true;
            _fireObj.transform.localPosition = CandleTypeMain == 0 ?
                new Vector3(0, _candleHeight0, 0) : new Vector3(0, _candleHeight1, 0);
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
        foreach (var row in _meshR)
        {
            foreach (var mesh in row)
            {
                mesh.enabled = false;
            }
        }
    }
}
