
using UdonSharp;
using VRC.SDKBase;

public class BurgerVendingPickupSub : UdonSharpBehaviour
{
    public BurgerVendingPickupMain _main;

    public override void OnPickup()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (_main != null) _main.MainPickup();
    }

    public override void OnDrop()
    {
        if (_main != null) _main.MainDrop();
    }

    public override void OnPickupUseDown()
    {
        if (_main != null) _main.MainPickupUseDown();
    }
}
