
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Bunkbeds_L_Curtains_L01_Gimmick : UdonSharpBehaviour
{
    [SerializeField] Transform _pickup;
    [SerializeField] SkinnedMeshRenderer _smr;
    [SerializeField] Vector3 _limitPos;
    Vector3 _pos;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ShapeKeyFloat))] float _shapeFloat = 0;

    public float ShapeKeyFloat
    {
        get => _shapeFloat;
        set
        {
            _shapeFloat = value;
            _smr.SetBlendShapeWeight(0, _shapeFloat);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            _pos = _pickup.localPosition;
            _pickup.localPosition = new Vector3(_pos.x, 0f, 0f);
            _pickup.localRotation = Quaternion.Euler(Vector3.zero);
            if (_pickup.localPosition.x < 0)
            {
                _pickup.localPosition = new Vector3(0f, 0f, 0f);
            }
            else if (_limitPos.x < _pickup.localPosition.x)
            {
                _pickup.localPosition = new Vector3(_limitPos.x, 0f, 0f);
            }
            float f = ((_limitPos.x - _pickup.localPosition.x) / _limitPos.x) * 100f;
            if (f != ShapeKeyFloat)
            {
                ShapeKeyFloat = f;
                RequestSerialization();
            }
        }
    }

}
