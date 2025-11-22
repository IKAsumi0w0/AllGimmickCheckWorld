
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class MoneyTreeMain : UdonSharpBehaviour
{
    [SerializeField] Animator _anime;
    [SerializeField] float _hitPSMarginTime;
    [SerializeField] AudioSource _moneySE;
    [SerializeField] AudioSource _se0;
    [SerializeField] AudioSource _limitTimerSE;
    [SerializeField] AudioSource _limitTimer2xSE;
    [SerializeField] MoneyTreeObjPickup_Sub[] _pickup0;
    [SerializeField] float _r0;
    [SerializeField] Transform _small;
    [SerializeField] float _r1;
    [SerializeField] float _randomR1Width;
    int _objCount0 = 0;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(Anime1stFlg))] bool _anime1stFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(Anime30sFlg))] bool _anime30sFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(AnimeFloat))] float _animeFloat = 0;

    public bool Anime1stFlg
    {
        get => _anime1stFlg;
        set
        {
            _anime1stFlg = value;
            _anime.SetBool("1stFlg", Anime1stFlg);
        }
    }

    public bool Anime30sFlg
    {
        get => _anime30sFlg;
        set
        {
            _anime30sFlg = value;
            _anime.SetBool("30sFlg", Anime30sFlg);
        }
    }

    public float AnimeFloat
    {
        get => _animeFloat;
        set
        {
            _animeFloat = value;
            if (_hitPSMarginTime < _animeFloat) _animeFloat = _hitPSMarginTime;
            if (_animeFloat < 0) _animeFloat = 0;
        }
    }

    void Start()
    {
        _objCount0 = _pickup0.Length;
    }

    void Update()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject) && !Anime30sFlg)
        {
            if (0 < AnimeFloat)
            {
                AnimeFloat -= Time.deltaTime;
                if (0 < AnimeFloat) Anime1stFlg = true;
                else Anime1stFlg = false;
                RequestSerialization();
            }
            else
            {
                Anime1stFlg = false;
                RequestSerialization();
            }
        }
    }

    public void ShowPickupObj()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            for (int i = 0; i < _objCount0; i++)
            {
                // 角度を計算（等間隔）
                float angle = i * Mathf.PI * 2f / _objCount0;
                float x = Mathf.Cos(angle) * _r0;
                float z = Mathf.Sin(angle) * _r0;

                // オブジェクトを生成
                Vector3 pos = new Vector3(x, 0, z);
                _pickup0[i].transform.localPosition = pos;
                _pickup0[i].transform.localRotation = Random.rotation;
                _pickup0[i]._main.FuncResetRigi();
                _pickup0[i]._main.DelayFuncMeshR_ON();
            }
            for (int i = 0; i < _small.childCount; i++)
            {
                // 角度を計算（等間隔）
                float angle = i * Mathf.PI * 2f / _small.childCount;
                float random = Random.Range(_r1 - _randomR1Width, _r1 + _randomR1Width);
                float x = Mathf.Cos(angle) * random;
                float z = Mathf.Sin(angle) * random;

                // オブジェクトを生成
                Vector3 pos = new Vector3(x, 0, z);
                _small.GetChild(i).transform.localPosition = pos;
                _small.GetChild(i).transform.localRotation = Random.rotation;
                MoneyTreeObjPickup_Sub mt = _small.GetChild(i).GetComponent<MoneyTreeObjPickup_Sub>();
                if (mt != null)
                {
                    mt._main.FuncResetRigi();
                    mt._main.DelayFuncMeshR_ON();
                }
            }
        }
        SendCustomEventDelayedSeconds(nameof(HidePickupObj), 30f, VRC.Udon.Common.Enums.EventTiming.Update);
    }

    public void HidePickupObj()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            for (int i = 0; i < _objCount0; i++)
            {
                _pickup0[i]._main.Reset();
            }
            for (int i = 0; i < _small.childCount; i++)
            {
                MoneyTreeObjPickup_Sub mt = _small.GetChild(i).GetComponent<MoneyTreeObjPickup_Sub>();
                if (mt != null)
                {
                    mt._main.Reset();
                }
            }
        }
    }

    //↓アニメーションイベントで使用中
    public void PlayMoneySE()
    {
        _moneySE.Play();
    }

    public void PlaySE0()
    {
        _se0.Play();
    }

    public void PlayLimitTimerSE()
    {
        _limitTimerSE.Play();
    }

    public void PlayLimitTimer2xSE()
    {
        _limitTimer2xSE.Play();
    }

    public void PlayAnime30s()
    {
        Anime30sFlg = true;
        RequestSerialization();
    }

    public void StopAnime30s()
    {
        Anime30sFlg = false;
        Anime1stFlg = false;
        AnimeFloat = 0;
        RequestSerialization();
    }
}
