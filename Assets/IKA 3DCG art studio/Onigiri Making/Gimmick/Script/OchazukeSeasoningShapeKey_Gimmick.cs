
using UdonSharp;
using UnityEngine;
using VRC.Economy;
using VRC.SDKBase;
using VRC.Udon;

public class OchazukeSeasoningShapeKey_Gimmick : UdonSharpBehaviour
{
    [SerializeField] GameObject _psObj;
    [SerializeField] float downwardThreshold = 0f;
    [SerializeField] SkinnedMeshRenderer _skinMeshR;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(PSFlg))] bool _psFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ShapekeyInt))] int _shapekeyInt = 0;

    public bool PSFlg
    {
        get => _psFlg;
        set
        {
            _psFlg = value;
            if (_psFlg) _psObj.SetActive(true);
            else _psObj.SetActive(false);
        }
    }

    public int ShapekeyInt
    {
        get => _shapekeyInt;
        set
        {
            _shapekeyInt = value;
            if (100 < _shapekeyInt)
            {
                _shapekeyInt = 0;
            }
            _skinMeshR.SetBlendShapeWeight(0, _shapekeyInt);
        }
    }

    void Update()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            if (ShapekeyInt == 100)
            {
                Vector3 objectUp = transform.up;
                float dotProduct = Vector3.Dot(objectUp, Vector3.up);
                if (dotProduct < downwardThreshold)
                {
                    PSFlg = true;
                    RequestSerialization();
                }
                else
                {
                    PSFlg = false;
                    RequestSerialization();
                }
            }
            else
            {
                PSFlg = false;
                RequestSerialization();
            }
        }
    }

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        for (int i = 0; i < _psObj.transform.childCount; i++)
        {
            Networking.SetOwner(Networking.LocalPlayer, _psObj.transform.GetChild(i).gameObject);
        }
    }

    public override void OnPickupUseDown()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            ShapekeyInt += 100;
            RequestSerialization();
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            PSFlg = PSFlg;
            RequestSerialization();
        }
    }

}
