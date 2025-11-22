
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class SBTSet : UdonSharpBehaviour
{
    [SerializeField] GameObject[] _obj = default;
    [SerializeField] GameObject _spoon = default;

    void Start()
    {
        Reset();
    }

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        for (int i = 0; i < _obj.Length; i++)
        {
            Networking.SetOwner(Networking.LocalPlayer, _obj[i]);
        }
        Networking.SetOwner(Networking.LocalPlayer, _spoon);
    }

    public override void OnPickupUseDown()
    {
        Reset();
    }

    public void Reset()
    {
        for (int i = 0; i < _obj.Length; i++)
        {
            SBTSet_Toast01 sBTSet_Toast01 = _obj[i].GetComponent<SBTSet_Toast01>();
            if (sBTSet_Toast01 != null)
            {
                sBTSet_Toast01.Respawn();
            }
            IKAPickupEatObjGimmick iKAPickupEatObjGimmick = _obj[i].GetComponent<IKAPickupEatObjGimmick>();
            if (iKAPickupEatObjGimmick != null)
            {
                iKAPickupEatObjGimmick.Reset();
            }
        }
        Spoon_SBT spon = _spoon.GetComponent<Spoon_SBT>();
        if (spon != null)
        {
            spon.Reset();
        }
    }
}
