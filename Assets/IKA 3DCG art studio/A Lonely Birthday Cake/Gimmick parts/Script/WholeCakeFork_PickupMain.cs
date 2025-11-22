
using System.Runtime.InteropServices;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Serialization.OdinSerializer;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class WholeCakeFork_PickupMain : UdonSharpBehaviour
{
    public WholeCakeFork_PickupSub _sub;
    [SerializeField] MeshRenderer _meshR;
    [SerializeField] Collider _mainColl;
    [SerializeField] Collider _subColl;
    [SerializeField] AudioSource _hitSE, _eatSE;
    [OdinSerialize] public GameObject[][] _cakeMeshR;

    [SerializeField] Text _debugText;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(DisplayFlg))] bool _displayFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(SetFlg))] bool _setFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CakeNoMain))] int _cakeNoMain = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CakeNoSub))] int _cakeNoSub = 0;


    public bool DisplayFlg
    {
        get => _displayFlg;
        set
        {
            _displayFlg = value;
            _meshR.enabled = _displayFlg;
            _mainColl.enabled = _displayFlg;
            _subColl.enabled = _displayFlg;
        }
    }
    public bool SetFlg
    {
        get => _setFlg;
        set
        {
            _setFlg = value;
            if (value)
            {
                _cakeMeshR[CakeNoMain][CakeNoSub].SetActive(_setFlg);
            }
            else
            {
                foreach (GameObject[] item0 in _cakeMeshR)
                {
                    foreach (GameObject item1 in item0)
                    {
                        item1.SetActive(_setFlg);
                    }
                }
            }
        }
    }
    public int CakeNoMain { get => _cakeNoMain; set => _cakeNoMain = value; }
    public int CakeNoSub { get => _cakeNoSub; set => _cakeNoSub = value; }

    void Update()
    {
        if (_debugText != null)
        {
            VRCPlayerApi owner = Networking.GetOwner(gameObject);
            VRCPlayerApi owner1 = Networking.GetOwner(gameObject.transform.parent.gameObject);
            string n = "";
            if (owner != null) n = owner.displayName;
            else n = "null";
            string n1 = "";
            if (owner1 != null) n1 = owner1.displayName;
            else n1 = "null";
            _debugText.text =
                $"SetFlg: {SetFlg}\n" +
                $"nameSub:{n1}\n" +
                $"name:{n}";
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
        if (SetFlg)
        {
            SetFlg = false;
            RequestSerialization();
            PlayEatSE();
        }
    }

    public void PlayHitSE() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlayHitSESub));
    public void PlayHitSESub() => _hitSE.Play();

    public void PlayEatSE() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlayEatSESub));
    public void PlayEatSESub() => _eatSE.Play();

    public void UpdateValue() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(UpdateValueSub));
    public void UpdateValueSub() => RequestSerialization();

    public void FuncDisplayFlg_ON() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(FuncDisplayFlg_ONSub));
    public void FuncDisplayFlg_ONSub()
    {
        DisplayFlg = true;
        RequestSerialization();
    }

    public void FuncDisplayFlg_OFF() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(FuncDisplayFlg_OFFSub));
    public void FuncDisplayFlg_OFFSub()
    {
        DisplayFlg = false;
        RequestSerialization();
    }

    public void Reset()
    {
        VRCPickup p = (VRCPickup)_sub.GetComponent(typeof(VRCPickup));
        if (p != null)
        {
            p.Drop();
        }
        DisplayFlg = false;
        SetFlg = false;
        CakeNoMain = 0;
        CakeNoSub = 0;
        _sub.gameObject.transform.localPosition = Vector3.zero;
        _sub.gameObject.transform.localRotation = Quaternion.identity;
        RequestSerialization();
    }
}
