
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BtnController : UdonSharpBehaviour
{
    public Image image;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(SelectFlg))] bool _selectFlg = false;

    public bool SelectFlg
    {
        get => _selectFlg;
        set
        {
            _selectFlg = value;
            image.enabled = _selectFlg;
        }
    }
}
