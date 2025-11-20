
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class IKA_FireworksTorch : UdonSharpBehaviour
{
    IKA_FireworksIgnition _ikaFwI;
    IKA_HandFireworksSusuki _ikaFwIS;
    void OnTriggerStay(Collider other)
    {
        _ikaFwI = other.GetComponent<IKA_FireworksIgnition>();
        if (_ikaFwI != null)
        {
            _ikaFwI.TogglePsObj = true;
        }
        _ikaFwIS = other.GetComponent<IKA_HandFireworksSusuki>();
        if (_ikaFwIS != null)
        {
            _ikaFwIS.TogglePsObj = true;
        }
        
    }
}
