
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Chopsticks_Gimmick : UdonSharpBehaviour
{
    public ChopsticksOpen_PickupSub[] _objs;
    public Transform _pool;
    [SerializeField] MeshRenderer _mr;
    [SerializeField] public GameObject _prefab;
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
            if (_objs[i]._main.SubCollState && _objs[i].transform.localPosition == Vector3.zero)
            {
                if (!ShowFlg)
                {
                    ShowFlg = true;
                    RequestSerialization();
                }
                return;
            }
        }
        for (int i = 0; i < _objs.Length; i++)
        {
            if (!_objs[i]._main.SubCollState)
            {
                if (!Networking.LocalPlayer.IsOwner(_objs[i].gameObject)) Networking.SetOwner(Networking.LocalPlayer, _objs[i].gameObject);
                if (!Networking.LocalPlayer.IsOwner(_objs[i]._main.gameObject)) Networking.SetOwner(Networking.LocalPlayer, _objs[i]._main.gameObject);

                _objs[i].transform.localPosition = Vector3.zero;
                _objs[i].transform.localRotation = Quaternion.identity;
                _objs[i]._main.SubCollState = true;
                _objs[i]._main.RequestSerialization();
                return;
            }
        }
        if (ShowFlg)
        {
            ShowFlg = false;
            RequestSerialization();
        }
    }
}
