
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class IKAForkTipGimmick : UdonSharpBehaviour
{
    IKAStabFoodGimmick _sfg = null;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(SetFlg))] bool _setFlg = false;

    public bool SetFlg
    {
        get => _setFlg;
        set
        {
            _setFlg = value;
            if (!_setFlg)
            {
                if (0 < transform.childCount)
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        Destroy(transform.GetChild(i).gameObject);
                    }
                }
                _sfg = null;
            }
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (_sfg == null)
        {
            _sfg = coll.gameObject.GetComponent<IKAStabFoodGimmick>();
            if (_sfg != null)
            {
                GameObject o = Instantiate(_sfg._stabObj, transform);
                o.transform.localPosition = Vector3.zero;
                o.transform.localRotation = Quaternion.Euler(Vector3.zero);
                SetFlg = true;
            }
        }
    }

    void OnTriggerStay(Collider coll)
    {
        if (_sfg == null)
        {
            _sfg = coll.gameObject.GetComponent<IKAStabFoodGimmick>();
            if (_sfg != null)
            {
                GameObject o = Instantiate(_sfg._stabObj, transform);
                o.transform.localPosition = Vector3.zero;
                o.transform.localRotation = Quaternion.Euler(Vector3.zero);
                SetFlg = true;
            }
        }
    }
}
