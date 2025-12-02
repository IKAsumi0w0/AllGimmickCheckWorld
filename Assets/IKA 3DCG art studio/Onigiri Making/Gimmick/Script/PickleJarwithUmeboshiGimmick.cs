
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PickleJarwithUmeboshiGimmick : UdonSharpBehaviour
{
    public Umebosshi_Pickup[] _objs;
    public Transform _pool;
    public GameObject _prefab;
    [SerializeField] VRCPickup _pickup;
    [SerializeField] OnigiriMakingCommonLid_Pickup _lid_Pickup;
    [SerializeField] GameObject _tuboFull;
    [SerializeField] GameObject _tuboEmpty;
    float _spawnTimer = 0f;
    float _spawnResetDelay = 0.5f;

    float _resetTimer = 0f;
    float _resetDelay1 = 5f;

    int _resetCount = 0;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(TuboFlg))] bool _tuboFlg = false;

    public bool TuboFlg
    {
        get => _tuboFlg;
        set
        {
            _tuboFlg = value;
            _tuboFull.SetActive(_tuboFlg);
            _tuboEmpty.SetActive(!_tuboFlg);
        }
    }

    public int ResetCount
    {
        get => _resetCount;
        set
        {
            _resetCount = value;
            if (3 <= _resetCount)
            {
                Reset();
                ResetCount = 0;
            }
        }
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

            if (0 < ResetCount)
            {
                _resetTimer += Time.deltaTime;
                if (_resetTimer >= _resetDelay1)
                {
                    ResetCount = 0;
                    _resetTimer = 0f;
                }
            }
        }
    }

    public override void Interact()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            ++ResetCount;
            _resetTimer = 0;
        }
        else
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            ResetCount = 0;
        }
    }

    public void SpawnObj()
    {
        for (int i = 0; i < _objs.Length; i++)
        {
            if (_objs[i]._main.MeshRFlg && _objs[i].transform.localPosition == Vector3.zero)
            {
                if (!TuboFlg)
                {
                    TuboFlg = true;
                    RequestSerialization();
                }
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
                if (!TuboFlg)
                {
                    TuboFlg = true;
                    RequestSerialization();
                }
                return;
            }
        }
        if (TuboFlg)
        {
            TuboFlg = false;
            RequestSerialization();
        }
    }

    public void Reset()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            if (!Networking.LocalPlayer.IsOwner(_lid_Pickup.gameObject)) Networking.SetOwner(Networking.LocalPlayer, _lid_Pickup.gameObject);
            _lid_Pickup.PickupState = false;
            _pickup.Drop();

            for (int i = 0; i < _objs.Length; i++)
            {
                if (!Networking.LocalPlayer.IsOwner(_objs[i].gameObject)) Networking.SetOwner(Networking.LocalPlayer, _objs[i].gameObject);
                if (!Networking.LocalPlayer.IsOwner(_objs[i]._main.gameObject)) Networking.SetOwner(Networking.LocalPlayer, _objs[i]._main.gameObject);
            }
            for (int i = 0; i < _objs.Length; i++)
            {
                _objs[i].gameObject.transform.localPosition = Vector3.zero;
                _objs[i].gameObject.transform.localRotation = Quaternion.identity;
                _objs[i]._main.MeshRFlg = false;
                _objs[i]._main.RequestSerialization();
            }
            if (!TuboFlg) TuboFlg = true;
            RequestSerialization();
        }
    }
}
