
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class hinge : UdonSharpBehaviour
{
    public Animator animator;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(hingeFlg))] private bool _flg = false;

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
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(HingeAnime));
    }

    public void HingeAnime()
    {
        if (hingeFlg) hingeFlg = false;
        else hingeFlg = true;
    }
}
