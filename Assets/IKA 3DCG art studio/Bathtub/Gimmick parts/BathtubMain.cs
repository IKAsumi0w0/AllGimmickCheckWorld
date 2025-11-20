
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BathtubMain : UdonSharpBehaviour
{
    public Animator animator; // アニメーターコンポーネント
    public string boolParameterName0; // アニメーションのBoolパラメーター名
    public string boolParameterName1; // アニメーションのBoolパラメーター名

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ToggleAnimeSwitch0))]
    bool _flg0 = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ToggleAnimeSwitch1))]
    bool _flg1 = false;

    public bool ToggleAnimeSwitch0
    {
        get => _flg0;
        set
        {
            _flg0 = value;
            if (animator != null && !string.IsNullOrEmpty(boolParameterName0))
            {
                animator.SetBool(boolParameterName0, _flg0); // Boolパラメーターを設定
            }
        }
    }

    public bool ToggleAnimeSwitch1
    {
        get => _flg1;
        set
        {
            _flg1 = value;
            if (animator != null && !string.IsNullOrEmpty(boolParameterName1))
            {
                animator.SetBool(boolParameterName1, _flg1); // Boolパラメーターを設定
            }
        }
    }

    public void FuncSub0()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            if (ToggleAnimeSwitch1) ToggleAnimeSwitch1 = false;
            ToggleAnimeSwitch0 = !ToggleAnimeSwitch0;
            RequestSerialization();
        }
    }

    public void FuncSub1()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            if (ToggleAnimeSwitch0) ToggleAnimeSwitch0 = false;
            ToggleAnimeSwitch1 = !ToggleAnimeSwitch1;
            RequestSerialization();
        }
    }

}
