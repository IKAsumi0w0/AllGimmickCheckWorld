
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IKA_Umbrella_Main : UdonSharpBehaviour
{
    [SerializeField] GameObject _psObj;
    [SerializeField] float _psOffsetH;
    [SerializeField] int _updateInterval = 5;
    [SerializeField] float _jitterChance = 0.1f;
    int _frameCounter;
    int _jitterOffset;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ToggleObj))] private bool _flg = false;

    public bool ToggleObj
    {
        get => _flg;
        set
        {
            _flg = value;
            _psObj.SetActive(_flg);
        }
    }

    void Update()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) return;

        if (!ToggleObj) return;

        _frameCounter++;

        if (_frameCounter % (_updateInterval + _jitterOffset) == 0)
        {
            _psObj.transform.position = transform.position + new Vector3(0, _psOffsetH, 0);
            // 1フレーム増やす処理
            _jitterOffset = Random.value < _jitterChance ? 1 : 0;
        }
    }

    public void MainPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        ToggleObj = true;
        RequestSerialization();
    }

    public void MainDrop()
    {
        ToggleObj = false;
        RequestSerialization();
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            ToggleObj = ToggleObj;
            RequestSerialization();
        }
    }

}
