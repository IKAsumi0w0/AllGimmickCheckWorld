
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Dial_poron : UdonSharpBehaviour
{
    [SerializeField] Transform _resetPos;
    [SerializeField] Dial_02 _dial_02;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ResetCount))] float _count = 0;

    public float ResetCount
    {
        get => _count;
        set
        {
            _count = value;
        }
    }

    void Update()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {

            ResetCount += Time.deltaTime;
            if (30f < ResetCount)
            {
                PoronReset();
            }
        }
    }

    public override void OnPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (!Networking.LocalPlayer.IsOwner(_dial_02.gameObject)) Networking.SetOwner(Networking.LocalPlayer, _dial_02.gameObject);

    }

    void OnTriggerStay(Collider other)
    {
        if ((other.gameObject.name.Contains("Dial_02")) && (Networking.LocalPlayer.IsOwner(other.gameObject)))
        {
            PoronReset();
        }
    }

    public void PoronReset()
    {
        ResetCount = 0;
        Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            this.gameObject.transform.position = _resetPos.position;
            this.gameObject.transform.rotation = _resetPos.rotation;
        }
        VRC_Pickup pickup = (VRC_Pickup)this.gameObject.GetComponent(typeof(VRC_Pickup));
        if (pickup != null)
        {
            pickup.Drop();
        }
        Dial_02 dial = _dial_02.gameObject.GetComponent<Dial_02>();
        if (dial != null)
        {
            dial.DropModelSwitch = false;
            dial.Dial02ModelSwitch = true;
        }
    }
}
