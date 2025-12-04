
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class hinge : UdonSharpBehaviour
{
    public Animator animator;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(hingeFlg))] bool _flg = false;

    public bool hingeFlg
    {
        get => _flg;
        set
        {
            _flg = value;
            animator.SetBool("lid_bool", _flg);
        }
    }

    public override void Interact()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        hingeFlg = !hingeFlg;
        RequestSerialization();
    }
}
