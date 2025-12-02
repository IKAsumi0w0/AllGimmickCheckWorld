
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IKA_summerFestival_SyrupMain : UdonSharpBehaviour
{
    [SerializeField] GameObject _obj;
    float _timeCount = 0;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ObjState))] bool _state = false;

    public bool ObjState
    {
        get => _state;
        set
        {
            _state = value;
            _obj.SetActive(_state);
        }
    }

    public void MainPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    private void Update()
    {
        if (30f < Vector3.Angle(this.transform.up, Vector3.up))
        {
            if (_timeCount < 30f)
            {
                _timeCount += Time.deltaTime;
                if (!ObjState)
                {
                    ObjState = true;
                    RequestSerialization();
                }
            }
            else
            {
                if (ObjState)
                {
                    ObjState = false;
                    RequestSerialization();
                }
            }
        }
        else
        {
            if (ObjState)
            {
                ObjState = false;
                RequestSerialization();
            }
            if (_timeCount != 0) _timeCount = 0f;
        }

    }

}
