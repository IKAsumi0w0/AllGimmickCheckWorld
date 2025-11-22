
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PumpkinTeapot_PSController : UdonSharpBehaviour
{
    [SerializeField] GameObject _psObj;
    [SerializeField] float threshold = 0.2f;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(PSFlg))] bool _psFlg = false;

    public bool PSFlg
    {
        get => _psFlg;
        set
        {
            _psFlg = value;
            if (_psFlg) _psObj.SetActive(true);
            else _psObj.SetActive(false);
        }
    }

    void Update()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            Vector3 rightDirection = transform.right;
            Vector3 downDirection = Vector3.down;
            float dotProduct = Vector3.Dot(rightDirection, downDirection);
            if (dotProduct > threshold)
            {
                if (!PSFlg)
                {
                    PSFlg = true;
                    RequestSerialization();
                }
            }
            else
            {
                if (PSFlg)
                {
                    PSFlg = false;
                    RequestSerialization();
                }
            }
        }
    }

}
