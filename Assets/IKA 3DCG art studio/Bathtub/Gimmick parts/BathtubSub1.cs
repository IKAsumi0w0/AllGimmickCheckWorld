
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BathtubSub1 : UdonSharpBehaviour
{
    [SerializeField] BathtubMain _main; // アニメーターコンポーネント

    public override void Interact()
    {
        // ネットワークオーナーにイベントを送信
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(SwitchAnime));
    }

    public void SwitchAnime()
    {
        _main.FuncSub1();
    }
}
