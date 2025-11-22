
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HeartLongFork_Manager : UdonSharpBehaviour
{
    [HideInInspector] public HeartLongFork_PickupSub[] _objs;
    public Transform _pool;
    public GameObject _prefab;
    [SerializeField] MeshRenderer _mr;
    float _timer = 0f;
    float _resetDelay = 0.5f;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ShowFlg))] bool _showFlg = false;

    public bool ShowFlg
    {
        get => _showFlg;
        set
        {
            _showFlg = value;
            _mr.enabled = _showFlg;
        }
    }

    void Start()
    {
        ShowFlg = false;
    }

    void Update()
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

    public void SpawnObj()
    {
        for (int i = 0; i < _objs.Length; i++)
        {
            if (_objs[i]._main.MeshRFlg && _objs[i].transform.localPosition == Vector3.zero)
            {
                return;
            }
        }
        for (int i = 0; i < _objs.Length; i++)
        {
            if (!_objs[i]._main.MeshRFlg)
            {
                _objs[i].transform.localPosition = Vector3.zero;
                _objs[i].transform.localRotation = Quaternion.identity;
                _objs[i]._main.FuncMeshR_ON();
                RequestSerialization();
                return;
            }
        }
    }
}
