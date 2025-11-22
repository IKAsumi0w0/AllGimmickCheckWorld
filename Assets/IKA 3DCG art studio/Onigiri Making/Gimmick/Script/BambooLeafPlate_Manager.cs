
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BambooLeafPlate_Manager : UdonSharpBehaviour
{
    public BambooLeafPlate_Pickup[] _objs;
    public Transform _pool;
    [SerializeField] MeshRenderer _mr;
    float _spawnTimer = 0f;
    float _spawnResetDelay = 0.5f;

    void Start()
    {
        _mr.enabled = false;
    }

    void Update()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            _spawnTimer += Time.deltaTime;
            if (_spawnResetDelay <= _spawnTimer)
            {
                SpawnObj();
                _spawnTimer = 0f;
            }
        }
    }

    public void SpawnObj()
    {
        for (int i = 0; i < _objs.Length; i++)
        {
            if (_objs[i]._main.SubCollState && _objs[i].transform.localPosition == Vector3.zero)
            {
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
    }
}
