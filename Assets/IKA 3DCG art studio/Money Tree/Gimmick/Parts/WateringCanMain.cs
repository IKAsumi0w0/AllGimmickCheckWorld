
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class WateringCanMain : UdonSharpBehaviour
{
    [SerializeField] GameObject _psObj;
    [SerializeField] float threshold = 0.3f;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(PSFlg))] bool _psFlg = false;

    public bool PSFlg
    {
        get => _psFlg;
        set
        {
            _psFlg = value;
            _psObj.SetActive(_psFlg);
        }
    }

    void Update()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            Vector3 forwardDirection = transform.right;
            Vector3 downDirection = Vector3.down;
            float dotProduct = Vector3.Dot(forwardDirection, downDirection);
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

    public void MainPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }
}
