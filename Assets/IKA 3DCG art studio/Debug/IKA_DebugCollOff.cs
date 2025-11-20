
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class IKA_DebugCollOff : UdonSharpBehaviour
{
    [SerializeField] Collider _coll;
    void Start()
    {
        if (_coll != null) _coll.isTrigger = true;
    }
}
