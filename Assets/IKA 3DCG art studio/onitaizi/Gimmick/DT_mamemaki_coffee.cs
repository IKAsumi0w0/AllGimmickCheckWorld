
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class DT_mamemaki_coffee : UdonSharpBehaviour
{
    [SerializeField] GameObject _magazineObj;

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    public override void OnPickupUseDown()
    {
        //if (Networking.LocalPlayer.IsOwner(this.gameObject))
        //{
        //    Ignition();
        //}
        //else
        //{
        //    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Ignition));
        //}
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
