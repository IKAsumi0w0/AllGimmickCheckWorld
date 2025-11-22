
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SpringTreats_CandyBall_ManagerSub : UdonSharpBehaviour
{
    [SerializeField] protected MeshRenderer _meshR;

    void Start()
    {
        if (_meshR) _meshR.enabled = false;
    }
}
