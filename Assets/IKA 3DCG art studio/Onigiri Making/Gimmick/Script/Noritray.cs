
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class Noritray : UdonSharpBehaviour
{
    public Nori_Manager _nm;
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
                ResetCount = 0;
            }
        }
    }

    public override void Interact()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            ResetCount = 0;
        }
        else
        {
            ++ResetCount;
        }
        RequestSerialization();
    }

    public void Reset()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            for (int i = 0; i < _nm._objs.Length; i++)
            {
                _nm._objs[i]._main.FuncReset();
            }
            ResetCount = 0;
            RequestSerialization();
        }
    }
}
