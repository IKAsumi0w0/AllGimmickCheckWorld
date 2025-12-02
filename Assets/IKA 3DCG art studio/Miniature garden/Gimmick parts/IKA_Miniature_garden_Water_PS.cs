
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class IKA_Miniature_garden_Water_PS : UdonSharpBehaviour
{
    [SerializeField] private GameObject _model;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ModelSwitch))] private bool _flg = false;

    public bool ModelSwitch
    {
        get => _flg;
        set
        {
            _flg = value;
            _model.SetActive(_flg);
        }
    }

    public override void Interact()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        ModelSwitch = !ModelSwitch;
        RequestSerialization();
    }
}
