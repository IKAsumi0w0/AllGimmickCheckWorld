
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class RicePaddle_Manager : UdonSharpBehaviour
{
    [HideInInspector] public RicePaddle_Pickup[] _objs;
    public Transform _pool;
    [SerializeField] MeshRenderer _mr;
    float _timer = 0f;
    float _resetDelay = 0.5f;

    void Start()
    {
        _mr.enabled = false;
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
                if (!Networking.LocalPlayer.IsOwner(_objs[i].gameObject)) Networking.SetOwner(Networking.LocalPlayer, _objs[i].gameObject);
                if (!Networking.LocalPlayer.IsOwner(_objs[i]._main.gameObject)) Networking.SetOwner(Networking.LocalPlayer, _objs[i]._main.gameObject);

                _objs[i].transform.localPosition = Vector3.zero;
                _objs[i].transform.localRotation = Quaternion.identity;
                _objs[i]._main.MeshRFlg = true;
                _objs[i]._main.RequestSerialization();
                return;
            }
        }
    }
}
