
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SewingTableStick : UdonSharpBehaviour
{
    [SerializeField] GameObject _target;
    private float _dis = 0;

    void Start()
    {
        _dis = Vector3.Distance(this.gameObject.transform.position, _target.transform.position);
    }

    private void Update()
    {
        float tmpDis = Vector3.Distance(this.gameObject.transform.position, _target.transform.position);
        Vector3 scale = this.transform.localScale;
        this.gameObject.transform.localScale = new Vector3(scale.x, scale.y, tmpDis / _dis);
        this.gameObject.transform.forward = (_target.transform.position - this.transform.position).normalized;
    }
}
