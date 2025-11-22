
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class Condimentcontainerwithgreenonions : UdonSharpBehaviour
{
    [SerializeField] OnigiriMakingCommonLid_Pickup _lidObj;
    [SerializeField] Greenonioncondimentspoon _spoonObj;
    int _resetCount = 0;

    public int ResetCount
    {
        get => _resetCount;
        set
        {
            _resetCount = value;
            if (3 <= _resetCount)
            {
                Reset();
            }
        }
    }

    public override void Interact()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            ++ResetCount;
        }
        else
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            ResetCount = 0;
        }
    }

    public void Reset()
    {
        if (!Networking.LocalPlayer.IsOwner(_lidObj.gameObject)) Networking.SetOwner(Networking.LocalPlayer, _lidObj.gameObject);
        _lidObj.Reset();
        if (!Networking.LocalPlayer.IsOwner(_spoonObj.gameObject)) Networking.SetOwner(Networking.LocalPlayer, _spoonObj.gameObject);
        _spoonObj.Reset();
        ResetCount = 0;
    }

}
