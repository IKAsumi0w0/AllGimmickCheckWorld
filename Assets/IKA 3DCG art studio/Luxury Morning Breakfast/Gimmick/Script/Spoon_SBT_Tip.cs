
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Spoon_SBT_Tip : UdonSharpBehaviour
{
    [SerializeField] GameObject _obj;
    SmashedbeanandButter _sb = null;
    SBTSet_Toast01 _st = null;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(SetFlg))] bool _setFlg = false;

    public bool SetFlg
    {
        get => _setFlg;
        set
        {
            _setFlg = value;
            _obj.SetActive(_setFlg);
        }
    }


    void Start()
    {
        SetFlg = false;
    }

    void OnTriggerEnter(Collider coll)
    {
        _sb = coll.gameObject.GetComponent<SmashedbeanandButter>();
        if (_sb != null)
        {
            SetFlg = true;
        }
        _st = coll.gameObject.GetComponent<SBTSet_Toast01>();
        if (_st != null)
        {
            if (SetFlg)
            {
                _st.SetFlg = true;
            }
            SetFlg = false;
        }
    }

    void OnTriggerStay(Collider coll)
    {
        if (_sb == null)
        {
            _sb = coll.gameObject.GetComponent<SmashedbeanandButter>();
            if (_sb != null)
            {
                SetFlg = true;
            }
        }
        if (_st == null)
        {
            _st = coll.gameObject.GetComponent<SBTSet_Toast01>();
            if (_st != null)
            {
                if (SetFlg)
                {
                    _st.SetFlg = true;
                }
                SetFlg = false;
            }
        }
    }
}
