
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class IKAPickupGimmick : UdonSharpBehaviour
{
    [SerializeField] IKAPickupEatObjGimmick[] _iKAPickupEatObjGimmick = default;

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        for (int i = 0; i < _iKAPickupEatObjGimmick.Length; i++)
        {
            Networking.SetOwner(Networking.LocalPlayer, _iKAPickupEatObjGimmick[i].gameObject);
        }
    }

    public override void OnPickupUseDown()
    {
        for (int i = 0; i < _iKAPickupEatObjGimmick.Length; i++)
        {
            _iKAPickupEatObjGimmick[i].Reset();
        }
    }
}
