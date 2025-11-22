
using UdonSharp;
using UnityEngine;
using UnityEngine.Animations;
using VRC.SDKBase;
using VRC.Udon;

public class PumpkinTeapot_Gimmick : UdonSharpBehaviour
{
    [SerializeField] GameObject _psObj;

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        Networking.SetOwner(Networking.LocalPlayer, _psObj);
    }
}
