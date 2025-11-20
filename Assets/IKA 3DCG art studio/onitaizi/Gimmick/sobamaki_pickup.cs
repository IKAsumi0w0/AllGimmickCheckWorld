
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class sobamaki_pickup : UdonSharpBehaviour
{
    [SerializeField] GameObject _magazineObj;
    [SerializeField] GameObject _sobaObj;
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
        int rnd = Random.Range(0, 10);
        if (rnd != 0)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Ignition1));
        }
        else
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(Ignition2));

        }
    }

    public void Ignition1()
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

    public void Ignition2()
    {
        _sobaObj.SetActive(true);
    }
}
