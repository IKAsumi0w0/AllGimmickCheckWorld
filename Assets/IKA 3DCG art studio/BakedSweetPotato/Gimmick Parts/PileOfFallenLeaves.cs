
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PileOfFallenLeaves : UdonSharpBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if (Networking.LocalPlayer.IsOwner(other.gameObject))
        {
            ImoMainColl imoMainColl = other.GetComponent<ImoMainColl>();
            if (imoMainColl != null && !imoMainColl._woodenStickMain.ImoMainDisplayState)
            {
                imoMainColl._woodenStickMain.TrueImoMainDisplay();
            }
        }
    }
}
