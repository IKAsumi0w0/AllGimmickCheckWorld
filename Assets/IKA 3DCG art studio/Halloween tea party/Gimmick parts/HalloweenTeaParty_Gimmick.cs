
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class HalloweenTeaParty_Gimmick : UdonSharpBehaviour
{
    [SerializeField] SkinnedMeshRenderer _skinMeshR;
    [SerializeField] GameObject _psObj;
    [SerializeField] HalloweenTeaParty_Coll _htpc;
    bool _pickupUseFlg = false;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ShapekeyFloat))] float _shapekeyFloat = 100;

    public float ShapekeyFloat
    {
        get => _shapekeyFloat;
        set
        {
            _shapekeyFloat = value;
            if (_shapekeyFloat < 0) _shapekeyFloat = 0;
            else if (100 < _shapekeyFloat) _shapekeyFloat = 100;
            if (_shapekeyFloat < 80) _psObj.SetActive(true);
            else _psObj.SetActive(false);
            _skinMeshR.SetBlendShapeWeight(0, _shapekeyFloat);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            if (_pickupUseFlg)
            {
                ShapekeyFloat += 0.5f;
            }
            RequestSerialization();
        }
    }

    public void MainPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        Networking.SetOwner(Networking.LocalPlayer, _htpc.gameObject);
    }

    public void MainDrop()
    {
        _pickupUseFlg = false;
    }

    public void MainPickupUseDown()
    {
        _pickupUseFlg = true;
    }

    public void MainPickupUseUp()
    {
        _pickupUseFlg = false;
    }

}
