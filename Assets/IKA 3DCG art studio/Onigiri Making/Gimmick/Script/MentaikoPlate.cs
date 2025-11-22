
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MentaikoPlate : UdonSharpBehaviour
{
    public Mentaiko_Manager _mm;
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
            for (int i = 0; i < _mm._objs.Length; i++)
            {
                _mm._objs[i]._main.FuncReset();
            }
            ResetCount = 0;
            RequestSerialization();
        }
    }
}
