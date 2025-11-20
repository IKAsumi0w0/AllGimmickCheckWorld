
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class WoodenStickReset : UdonSharpBehaviour
{
    void OnTriggerStay(Collider other)
    {
        WoodenStickMain woodenStickMain = other.GetComponent<WoodenStickMain>();
        if (woodenStickMain != null)
        {
            woodenStickMain.Reset();
        }
    }
}
