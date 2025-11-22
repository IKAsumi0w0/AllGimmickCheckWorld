
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ChocolateFountainReset : UdonSharpBehaviour
{
    [SerializeField] GameObject _clearColl;

    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ShowClearColl)); 
    }

    public void ShowClearColl()
    {
        _clearColl.SetActive(true);
    }

}
