
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class hot_watertap_BK : UdonSharpBehaviour
{
    [SerializeField] private Animator _anime;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(AnimeSwitch))] private bool _flg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(AnimeFloat))] private float _count = 0;
    float _delayCount = 0;

    public bool AnimeSwitch
    {
        get => _flg;
        set
        {
            _flg = value;
            _anime.SetBool("HotWaterSwitch", _flg);
        }
    }

    public float AnimeFloat
    {
        get => _count;
        set
        {
            _count = value;
            _anime.SetFloat("BathtubWaterFloat", _count);
        }
    }

    void Start()
    {

    }

    void Update()
    {
        if (Networking.LocalPlayer.IsOwner(this.gameObject))
        {

            if (AnimeSwitch)
            {
                _delayCount = 0;
                if (AnimeFloat <= 1f)
                {
                    AnimeFloat += Time.deltaTime / 10f;
                }
            }
            else
            {
                if (30f < _delayCount)
                {
                    if (0 <= AnimeFloat)
                    {
                        AnimeFloat -= Time.deltaTime / 10f;
                    }
                }
                else
                {
                    _delayCount += Time.deltaTime;
                }
            }
        }
    }

    public override void Interact()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(ShowSwitch));
    }

    public void ShowSwitch()
    {
        if (AnimeSwitch)
        {
            AnimeSwitch = false;
        }
        else
        {
            AnimeSwitch = true;
        }
    }
}
