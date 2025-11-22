
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;


public class MochiSpawn : UdonSharpBehaviour
{
    public Mochi[] _mochiArr;
    [SerializeField] int _probability = 6;

    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(SpawnMochi));
    }

    public void SpawnMochi()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            for (int i = 0; i < _mochiArr.Length; i++)
            {
                if (!_mochiArr[i]._main._coll.enabled)
                {
                    Networking.SetOwner(Networking.LocalPlayer, _mochiArr[i].gameObject);
                    Networking.SetOwner(Networking.LocalPlayer, _mochiArr[i]._main.gameObject);
                    _mochiArr[i]._rigi.velocity = Vector3.zero;
                    _mochiArr[i]._rigi.angularVelocity = Vector3.zero;
                    _mochiArr[i].transform.localPosition = Vector3.zero;
                    _mochiArr[i].transform.localRotation = Quaternion.identity;
                    _mochiArr[i]._main.ShowMochi();
                    break;
                }
            }
        }
    }

    public void AllReSpawnMochi()
    {
        for (int i = 0; i < _mochiArr.Length; i++)
        {
            _mochiArr[i]._main.FlgSwitchInt = Random.Range(0, _probability); ;
            _mochiArr[i]._main.ResetFlg = true;
            RequestSerialization();
        }
    }
}
