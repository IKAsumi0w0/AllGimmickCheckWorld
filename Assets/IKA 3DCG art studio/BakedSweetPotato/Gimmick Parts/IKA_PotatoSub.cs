
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class IKA_PotatoSub : UdonSharpBehaviour
{
    [SerializeField] WoodenStickMain _woodenStickMain;
    [SerializeField] BoxCollider _coll;
    [SerializeField] AudioSource _eatSE;
    [SerializeField] GameObject[] _imoSubObjArr;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ImoSubDisplayState))] int _imoSubState = 0;

    public int ImoSubDisplayState
    {
        get => _imoSubState;
        set
        {
            _imoSubState = value;
            foreach (GameObject item in _imoSubObjArr) if (item.activeSelf) item.SetActive(false);
            if (value == 0) _coll.enabled = false;
            else if (!_coll.enabled) _coll.enabled = true;
            if (value == 1)
            {
                _imoSubObjArr[0].SetActive(true);
            }
            else if (value == 2)
            {
                _imoSubObjArr[1].SetActive(true);
            }
            else if (value == 3)
            {
                _imoSubObjArr[2].SetActive(true);
            }
        }
    }

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    public override void OnPickupUseDown()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(EatImoSub));
    }

    public void EatImoSub()
    {
        _eatSE.Play();
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            if (1 == ImoSubDisplayState)
            {
                ImoSubDisplayState = 2;
            }
            else if (2 <= ImoSubDisplayState)
            {

                ImoSubDisplayState = 0;
                VRCPickup p = (VRCPickup)GetComponent(typeof(VRCPickup));
                if (p != null)
                {
                    p.Drop();
                }
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(DelayReset));
            }
            RequestSerialization();
        }
    }

    public void DelayReset() => SendCustomEventDelayedSeconds(nameof(Reset), 2f, VRC.Udon.Common.Enums.EventTiming.Update);

    public void Reset()
    {

        if (Networking.LocalPlayer.IsOwner(_woodenStickMain.gameObject))
        {
            _woodenStickMain.CheckReset();
        }
    }
}
