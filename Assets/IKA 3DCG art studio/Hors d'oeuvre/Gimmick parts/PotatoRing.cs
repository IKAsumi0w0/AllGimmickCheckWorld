using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class PotatoRing : UdonSharpBehaviour
{
    public Animator _animator;
    public bool _flg = false;

    void Start()
    {
        _animator = this.GetComponent<Animator>();
    }

    public override void OnPickup()
    {
        if (_flg == false)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Initialize");
            _flg = true;
        }
    }

    public override void OnDrop()
    {
        if (_flg == false)
        {
            this.gameObject.transform.localPosition = Vector3.zero;
            this.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }

    public void Initialize()
    {
        _animator.SetTrigger("Initialize");
    }

    public void ReSpawn()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ReSpawnMethod");
    }

    public void ReSpawnMethod()
    {
        VRCPickup pickup1 = (VRCPickup)gameObject.GetComponent(typeof(VRCPickup));
        if (pickup1 != null) pickup1.Drop();
        _animator.SetTrigger("ReSpawn");
        this.gameObject.transform.localPosition = Vector3.zero;
        this.gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
        _flg = false;
    }
}
