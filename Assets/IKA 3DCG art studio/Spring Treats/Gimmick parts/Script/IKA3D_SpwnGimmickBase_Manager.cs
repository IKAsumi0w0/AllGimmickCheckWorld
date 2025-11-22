
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IKA3D_SpwnGimmickBase_Manager : UdonSharpBehaviour
{
    [HideInInspector] public IKA3D_SpwnGimmickBase_PickupSub[] _objs;
    public Transform _pool;
    public GameObject _prefab;
    [SerializeField] protected MeshRenderer _mr;
    [SerializeField] protected GameObject _meshObj;
    protected float _timer = 0f;
    protected float _resetDelay = 0.5f;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ShowFlg))] protected bool _showFlg = false;

    public virtual bool ShowFlg
    {
        get => _showFlg;
        set
        {
            _showFlg = value;
            if (_mr) _mr.enabled = _showFlg;
            if (_meshObj) _meshObj.SetActive(_showFlg);
        }
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            _timer += Time.deltaTime;
            if (_resetDelay <= _timer)
            {
                SpawnObj();
                _timer = 0f;
            }
        }
    }

    public virtual void SpawnObj()
    {
        foreach (var obj in _objs)
        {
            if (obj._main.DisplayFlg && obj.transform.localPosition == Vector3.zero)
            {
                if (!ShowFlg)
                {
                    ShowFlg = true;
                    RequestSerialization();
                }
                return;
            }
        }

        foreach (var obj in _objs)
        {
            if (!obj._main.DisplayFlg && obj.transform.localPosition == Vector3.zero)
            {
                obj._main.FuncDisplayFlg_ON();
                RequestSerialization();
                return;
            }
        }

        ShowFlg = false;
        RequestSerialization();
    }
}
