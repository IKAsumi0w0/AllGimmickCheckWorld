
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IKA_WoodenSticks : UdonSharpBehaviour
{


    public WoodenStickMain[] _objs;
    public Transform _pool;
    public GameObject _prefab;
    [SerializeField] MeshRenderer _mr;
    [Header("=====爆発確率(%)=====")]
    [SerializeField] float _explosionProbability = 10;
    float _timer = 0f;
    float _resetDelay = 0.5f;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(DisplayState))] bool _displayState = false;

    public bool DisplayState
    {
        get => _displayState;
        set
        {
            _displayState = value;
            if (_mr) _mr.enabled = _displayState;
        }
    }

    void Start()
    {
        foreach (var obj in _objs)
        {
            obj._explosionProbability = _explosionProbability;
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
        foreach (var obj in _objs)
        {
            if (obj.WoodenStickState && obj._woodenStickObj.transform.localPosition == Vector3.zero)
            {
                if (!DisplayState)
                {
                    DisplayState = true;
                    RequestSerialization();
                }
                return;
            }
        }

        foreach (var obj in _objs)
        {
            if (!obj.WoodenStickState && obj._woodenStickObj.transform.localPosition == Vector3.zero)
            {
                obj.TrueWoodenStick();
                obj.SetExplosionState();
                return;
            }
        }

        DisplayState = false;
        RequestSerialization();
    }
}
