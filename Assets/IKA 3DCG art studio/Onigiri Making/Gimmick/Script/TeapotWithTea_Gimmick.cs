
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
public class TeapotWithTea_Gimmick : UdonSharpBehaviour
{
    [SerializeField] GameObject _psObj;
    [SerializeField] OnigiriMakingCommonLid_Pickup _lid_Pickup;     
    [SerializeField] float threshold = 0.2f;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(PSFlg))] bool _psFlg = false;

    public bool PSFlg
    {
        get => _psFlg;
        set
        {
            _psFlg = value;
            if (_psFlg)
            {
                _psObj.SetActive(true);
            }
            else _psObj.SetActive(false);
        }
    }

    void Update()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            Vector3 forwardDirection = transform.forward;
            Vector3 downDirection = Vector3.down;
            float dotProduct = Vector3.Dot(forwardDirection, downDirection);
            if (dotProduct > threshold)
            {
                if (!PSFlg)
                {
                    PSFlg = true;
                    RequestSerialization();
                }
            }
            else
            {
                if (PSFlg)
                {
                    PSFlg = false;
                    RequestSerialization();
                }
            }
        }
    }

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        Networking.SetOwner(Networking.LocalPlayer, _psObj);
        if (!_lid_Pickup.PickupState) Networking.SetOwner(Networking.LocalPlayer, _lid_Pickup.gameObject);
    }
}
