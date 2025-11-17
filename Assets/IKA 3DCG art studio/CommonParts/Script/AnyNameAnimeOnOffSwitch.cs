
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class AnyNameAnimeOnOffSwitch : UdonSharpBehaviour
{
    public Animator animator; // アニメーターコンポーネント
    public string boolParameterName = ""; // アニメーションのBoolパラメーター名

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ToggleAnimeSwitch))]
    bool _flg = false;

    public bool ToggleAnimeSwitch
    {
        get => _flg;
        set
        {
            _flg = value;

            if (animator != null && !string.IsNullOrEmpty(boolParameterName))
            {
                animator.SetBool(boolParameterName, _flg); // Boolパラメーターを設定
            }
        }
    }

    public override void Interact()
    {
        // ネットワークオーナーにイベントを送信
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(SwitchAnime));
    }

    public void SwitchAnime()
    {
        // アニメーションのON/OFFを切り替える
        ToggleAnimeSwitch = !ToggleAnimeSwitch;
        RequestSerialization();
    }
}
