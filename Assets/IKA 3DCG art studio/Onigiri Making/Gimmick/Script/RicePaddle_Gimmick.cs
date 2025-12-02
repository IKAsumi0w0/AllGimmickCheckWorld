
using Newtonsoft.Json.Linq;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class RicePaddle_Gimmick : UdonSharpBehaviour
{
    public int _paddleNo;
    public RiceSet_Manager _setM;
    public RicePaddle_Pickup _sub;
    public Transform _setPosTrans;
    [SerializeField] BoxCollider _collSub;
    [SerializeField] BoxCollider _collMain;
    [SerializeField] MeshRenderer _mr;
    float _timer = 0f;
    float _resetDelay = 3f;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(MeshRFlg))] bool _meshRFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(RiceBallState))] bool _riceBallState = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ResetCount))] int _resetCount = 0;

    public bool MeshRFlg
    {
        get => _meshRFlg;
        set
        {
            _meshRFlg = value;
            _mr.enabled = _meshRFlg;
            _collSub.enabled = _meshRFlg;
            _collMain.enabled = _meshRFlg;
        }
    }

    public bool RiceBallState
    {
        get => _riceBallState;
        set
        {
            _riceBallState = value;
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

    void Update()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            if (0 < ResetCount)
            {
                _timer += Time.deltaTime;
                if (_timer >= _resetDelay)
                {
                    ResetCount = 0;
                    _timer = 0f;
                }
            }
        }
        else
        {
            for (int i = 0; i < _setPosTrans.childCount; i++)
            {
                _setPosTrans.GetChild(i).localPosition = Vector3.zero;
                _setPosTrans.GetChild(i).localRotation = Quaternion.identity;
            }
        }
    }

    public void MainPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    public void MainPickupUseDown()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            ++ResetCount;
            _timer = 0;
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            MeshRFlg = MeshRFlg;
            RequestSerialization();
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        RiceCookerWithRice_Gimmick rpp = coll.gameObject.GetComponent<RiceCookerWithRice_Gimmick>();
        if (rpp && !RiceBallState && Networking.LocalPlayer.IsOwner(gameObject))
        {
            for (int i = 0; i < _setM._rbm._objs.Length; i++)
            {
                if (!_setM._rbm._objs[i]._main.MeshRFlg)
                {
                    Networking.SetOwner(Networking.LocalPlayer, _setM._rbm._objs[i].gameObject);
                    Networking.SetOwner(Networking.LocalPlayer, _setM._rbm._objs[i]._main.gameObject);
                    _setM._rbm._objs[i]._main.MeshRFlg = true;
                    _setM._rbm._objs[i]._main.PaddleNo = _paddleNo;
                    _setM._rbm._objs[i]._main.RequestSerialization();
                    RiceBallState = true;
                    RequestSerialization();
                    break;
                }
            }
        }
    }

    public void RicePaddleToReset()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(RicePaddleToResetSub));
    }

    public void Reset()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            for (int i = 0; i < _setM._rbm._objs.Length; i++)
            {
                if (_setM._rbm._objs[i]._main.PaddleNo == _paddleNo)
                {
                    if (!Networking.LocalPlayer.IsOwner(_setM._rbm._objs[i]._main.gameObject)) Networking.SetOwner(Networking.LocalPlayer, _setM._rbm._objs[i]._main.gameObject);
                    _setM._rbm._objs[i]._main.Reset();
                }
            }
            VRCPickup pickup = (VRCPickup)_sub.gameObject.GetComponent(typeof(VRCPickup));
            if (pickup != null)
            {
                pickup.Drop();
            }
            _sub.gameObject.transform.localPosition = Vector3.zero;
            _sub.gameObject.transform.localRotation = Quaternion.identity;
            MeshRFlg = false;
            RiceBallState = false;
            RequestSerialization();
        }
    }

    public void RicePaddleToResetSub()
    {
        VRCPickup pickup = (VRCPickup)_sub.gameObject.GetComponent(typeof(VRCPickup));
        if (pickup != null)
        {
            pickup.Drop();
        }
        SendCustomEventDelayedSeconds(nameof(ResetPos), 0.5f, VRC.Udon.Common.Enums.EventTiming.Update);
        MeshRFlg = false;
        RiceBallState = false;
        RequestSerialization();
    }

    public void ResetPos()
    {
        _sub.gameObject.transform.localPosition = Vector3.zero;
        _sub.gameObject.transform.localRotation = Quaternion.identity;
    }
}
