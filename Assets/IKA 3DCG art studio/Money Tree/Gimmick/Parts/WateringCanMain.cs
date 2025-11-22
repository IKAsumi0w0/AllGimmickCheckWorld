
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

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

    void Start()
    {

    }

    void Update()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {
            Vector3 forwardDirection = transform.right;
            Vector3 downDirection = Vector3.down;
            float dotProduct = Vector3.Dot(forwardDirection, downDirection);
            if (dotProduct > threshold) PSFlg = true;
            else PSFlg = false;
            RequestSerialization();
        }
    }

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

}
