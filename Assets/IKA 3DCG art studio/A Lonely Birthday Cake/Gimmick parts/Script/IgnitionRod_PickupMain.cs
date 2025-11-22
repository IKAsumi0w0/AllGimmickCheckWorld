
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IgnitionRod_PickupMain : UdonSharpBehaviour
{
    public IgnitionRod_PickupSub _sub;
    [SerializeField] GameObject _fireObj;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(IgnitionFlg))] bool _ignitionFlg = false;

    public bool IgnitionFlg
    {
        get => _ignitionFlg;
        set
        {
            _ignitionFlg = value;
            _fireObj.SetActive(_ignitionFlg);
        }
    }

    public void MainPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    public void MainDrop()
    {

    }

    public void MainPickupUseDown()
    {
        IgnitionFlg = !IgnitionFlg;
        RequestSerialization();
    }

    public void Reset()
    {
        VRCPickup p = (VRCPickup)_sub.GetComponent(typeof(VRCPickup));
        if (p != null)
        {
            p.Drop();
        }
        _sub.transform.position = new Vector3(0, -10000f, 0);
        _sub.transform.rotation = Quaternion.identity;
        IgnitionFlg = false;
        RequestSerialization();
    }
}
