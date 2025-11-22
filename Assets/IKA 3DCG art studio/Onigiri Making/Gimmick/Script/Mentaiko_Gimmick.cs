
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Mentaiko_Gimmick : UdonSharpBehaviour
{
    [SerializeField] Mentaiko_Pickup _sub;
    [SerializeField] Collider _collSub;
    [SerializeField] Collider _collMain;
    [SerializeField] MeshRenderer _mr;
    [SerializeField] VRCPickup _pickup;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(MeshRFlg))] bool _meshRFlg = false;

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

    public void MainPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    void OnTriggerEnter(Collider coll)
    {
        Ochazuke_Pickup ocha = coll.GetComponent<Ochazuke_Pickup>();
        if (ocha != null && Networking.LocalPlayer.IsOwner(ocha.gameObject))
        {
            if (ocha._main.TypeNo == 1)
            {
                ocha._main.TypeNo = 3;
                ocha._main.RequestSerialization();
                Reset();
                RequestSerialization();
            }
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

    public void FuncReset()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Reset));
    }

    public void Reset()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            _pickup.Drop();
            _sub.gameObject.transform.localPosition = Vector3.zero;
            _sub.gameObject.transform.localRotation = Quaternion.identity;
            MeshRFlg = false;
            RequestSerialization();
        }
    }
}
