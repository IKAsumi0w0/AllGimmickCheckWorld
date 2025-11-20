
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class mamemaki_pickup : UdonSharpBehaviour
{
    [SerializeField] GameObject _magazineObj;
    [SerializeField] Transform _resetPosObj;

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    public override void OnDrop()
    {
        this.gameObject.transform.position = _resetPosObj.position;
        this.gameObject.transform.rotation = _resetPosObj.rotation;
    }

    public override void OnPickupUseDown()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Ignition));
    }

    public void Ignition()
    {
        for (int i = 0; i < _magazineObj.transform.childCount; i++)
        {
            if (!_magazineObj.transform.GetChild(i).gameObject.activeSelf)
            {
                _magazineObj.transform.GetChild(i).gameObject.SetActive(true);
                return;
            }
        }
    }
}
