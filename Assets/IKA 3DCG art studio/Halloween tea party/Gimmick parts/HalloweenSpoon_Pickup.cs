
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
public class HalloweenSpoon_Pickup : UdonSharpBehaviour
{
    [SerializeField] MeshRenderer _mr;
    [SerializeField] HalloweenSpoon_CollGimmick _hscg;

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

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        Networking.SetOwner(Networking.LocalPlayer, _hscg.gameObject);
    }

    public override void OnPickupUseUp()
    {
        if (ShowFlg)
        {
            ShowFlg = false;
            RequestSerialization();
        }
    }

}
