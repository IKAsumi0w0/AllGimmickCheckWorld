
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class hot_watertap_BK : UdonSharpBehaviour
{
    [SerializeField] Animator _anime;
    // 任意フレームごとに処理
    [SerializeField] int _updateInterval = 5;
    // たまに1フレーム増やす確率（0〜1）
    [SerializeField] float _jitterChance = 0.1f;
    int _frameCounter;
    int _jitterOffset;
    float _localAnimeFloat = 0;
    float _delayCount = 0;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(AnimeSwitch))] bool _flg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(AnimeFloat))] float _count = 0;

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

    void Update()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            _frameCounter++;
            // 指定間隔 + ジッター に一致したときだけ処理
            if (_frameCounter % (_updateInterval + _jitterOffset) == 0)
            {
                // 1フレーム増やす処理
                _jitterOffset = Random.value < _jitterChance ? 1 : 0;
                if (_localAnimeFloat != AnimeFloat)
                {
                    _localAnimeFloat = AnimeFloat;
                    RequestSerialization();
                }
            }

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
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        AnimeSwitch = !AnimeSwitch;
        RequestSerialization();
    }
}
