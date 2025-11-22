
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Teacup_Gimmick : UdonSharpBehaviour
{
    [SerializeField] SkinnedMeshRenderer _skinMeshR;
    [SerializeField] GameObject _psObj;
    [SerializeField] Teacup_Pickup _sub;
    [SerializeField] BoxCollider _collSub;
    [SerializeField] BoxCollider _collMain;
    bool _pickupUseFlg = false;
    float _timer = 0f;
    float _resetDelay = 5f;
    int _resetCount = 0;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(MainCollState))] bool _mainCollState = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(SubCollState))] bool _subCollState = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ShapekeyFloat))] float _shapekeyFloat = 0;

    public bool MainCollState
    {
        get => _mainCollState;
        set
        {
            _mainCollState = value;
            _collMain.enabled = value;
        }
    }

    public bool SubCollState
    {
        get => _subCollState;
        set
        {
            _subCollState = value;
            _skinMeshR.enabled = value;
            _collSub.enabled = value;
        }
    }

    public float ShapekeyFloat
    {
        get => _shapekeyFloat;
        set
        {
            _shapekeyFloat = value;
            if (_shapekeyFloat < 0) _shapekeyFloat = 0;
            else if (100 < _shapekeyFloat) _shapekeyFloat = 100;
            if (80 < _shapekeyFloat) _psObj.SetActive(true);
            else _psObj.SetActive(false);
            _skinMeshR.SetBlendShapeWeight(0, _shapekeyFloat);
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
                _resetCount = 0;
            }
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            if (_pickupUseFlg)
            {
                ShapekeyFloat -= 0.5f;
                RequestSerialization();
            }
            if (0 < ResetCount)
            {
                _timer += Time.deltaTime;
                if (_timer >= _resetDelay)
                {
                    ResetCount = 0;
                    _timer = 0f;
                    RequestSerialization();
                }
            }
        }
    }

    public void MainPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        _pickupUseFlg = false;
        MainCollState = true;
        ResetCount = 0;
        RequestSerialization();
    }

    public void MainDrop()
    {
        _pickupUseFlg = false;
    }

    public void MainPickupUseDown()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            ++ResetCount;
            _timer = 0;
            _pickupUseFlg = true;
            RequestSerialization();
        }
    }

    public void MainPickupUseUp()
    {
        _pickupUseFlg = false;
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            MainCollState = MainCollState;
            SubCollState = SubCollState;
            ShapekeyFloat = ShapekeyFloat;
            RequestSerialization();
        }
    }

    public void Reset()
    {
        VRCPickup pickup = (VRCPickup)_sub.gameObject.GetComponent(typeof(VRCPickup));
        if (pickup != null)
        {
            pickup.Drop();
        }
        _sub.gameObject.transform.localPosition = Vector3.zero;
        _sub.gameObject.transform.localRotation = Quaternion.identity;
        _pickupUseFlg = false;
        MainCollState = false;
        SubCollState = false;
        ShapekeyFloat = 0;
        RequestSerialization();
    }
}
