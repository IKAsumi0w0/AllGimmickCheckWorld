using BestHTTP.Authentication;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;
using System;
using System.Linq;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using Random = UnityEngine.Random;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class WholeCake_PickupMain : UdonSharpBehaviour
{
    [Header("=====爆発確率[Explosion probability](%)=====")]
    [SerializeField] int _explosionProbability = 50;
    [Space(30)]
    public GameObject _prefab;
    public Transform _trans0, _trans1;
    public int _cakeNo = 0;
    public CakeNumCandleGimmick[] _candleNumObj;
    public CakeSpecialCandleGimmick _candleHBObj, _candleAnnivObj;
    [SerializeField] GameObject _meshRMain;
    [SerializeField] MeshRenderer[] _meshR;
    [SerializeField] MeshRenderer[] _meshR0;
    [SerializeField] Collider _subColl;
    [SerializeField] WholeCake_PickupSub _sub;
    [SerializeField] float _r0, _r1;

    [SerializeField] AudioSource _bgm_N, _bgm_Bom;
    [SerializeField] GameObject _bomPS;
    [SerializeField] BurntCake_PickupMain[] _burntChipObj;
    [SerializeField] float _wide = 0.04f;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(DisplayFlg))] bool _displayFlg = true;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CandleNormalCount))] int _candleNormalCount = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CandleNumCount))] int _candleNumCount = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CandleType))] int _candleType = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CandleNumFlg))] bool _candleNumFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CandleHappyBirthdayFlg))] bool _candleHBFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CandleAnnivFlg))] bool _candleAnnivFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(LightingCount))] int _lightingCount = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(LightingOptionCount))] int _lightingOptionCount = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CheckLightingFlg))] bool _checkLightingFlg = true;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ExplosionFlg))] bool _explosionFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(EatCount))] public int _eatCount = 8;

    public bool DisplayFlg
    { 
        get => _displayFlg;
        set
        {
            _displayFlg = value;
            _meshRMain.SetActive(_displayFlg);
            _subColl.enabled = _displayFlg;
        }
    }
    public int CandleNormalCount { get => _candleNormalCount; set => _candleNormalCount = value; }
    public int CandleNumCount { get => _candleNumCount; set => _candleNumCount = value; }
    public int CandleType { get => _candleType; set => _candleType = value; }
    public bool CandleNumFlg
    {
        get => _candleNumFlg;
        set
        {
            _candleNumFlg = value;
        }
    }
    public bool CandleHappyBirthdayFlg
    { 
        get => _candleHBFlg;
        set
        {
            _candleHBFlg = value;
        }
    }
    public bool CandleAnnivFlg
    {
        get => _candleAnnivFlg;
        set
        {
            _candleAnnivFlg = value;
        }
    }
    public int LightingCount { get => _lightingCount; set => _lightingCount = value; }
    public int LightingOptionCount { get => _lightingOptionCount; set => _lightingOptionCount = value; }
    public bool CheckLightingFlg { get => _checkLightingFlg; set => _checkLightingFlg = value; }
    public bool ExplosionFlg { get => _explosionFlg; set => _explosionFlg = value; }
    public int EatCount
    {
        get => _eatCount;
        set
        {
            _eatCount = value;
            foreach (MeshRenderer m in _meshR)
            {
                m.enabled = false;
            }
            if (8 <= _eatCount) _meshR[3].enabled = true;
            else if (6 <= _eatCount) _meshR[2].enabled = true;
            else if (3 <= _eatCount) _meshR[1].enabled = true;
            else if (1 <= _eatCount) _meshR[0].enabled = true;
            else DisplayFlg = false;
            if (0 < _meshR0.Length)
            {
                foreach (MeshRenderer m in _meshR0)
                {
                    m.enabled = false;
                }
                if (8 <= _eatCount) _meshR0[3].enabled = true;
                else if (6 <= _eatCount) _meshR0[2].enabled = true;
                else if (3 <= _eatCount) _meshR0[1].enabled = true;
                else if (1 <= _eatCount) _meshR0[0].enabled = true;
            }

        }
    }

    void Update()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject) && 
            !(0 == CandleNormalCount && !CandleNumFlg && !CandleHappyBirthdayFlg && !CandleAnnivFlg) &&

            !CheckLightingFlg && CandleNormalCount + LightingOptionCount <= LightingCount)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, ExplosionFlg ? nameof(PlayBomBGM) : nameof(PlayNormalBGM));
            CheckLightingFlg = true;
            RequestSerialization();
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        WholeCakeFork_PickupMain wcp = coll.GetComponent<WholeCakeFork_PickupMain>();
        if (wcp != null && Networking.LocalPlayer.IsOwner(wcp.gameObject) && !ExplosionFlg && !wcp.SetFlg && 1 <= EatCount)
        {
            wcp.CakeNoMain = _cakeNo;
            wcp.CakeNoSub = Random.Range(0, 2);
            wcp.UpdateValue();
            wcp.SetFlg = true;
            wcp.UpdateValue();
            EatCake();
            wcp.PlayHitSE();
        }
    }

    public void MainPickup()
    {

    }

    public void MainDrop()
    {

    }

    public void MainPickupUseDown()
    {

    }

    public void PlayBomBGM()
    {
        _bgm_Bom.Play();
        SendCustomEventDelayedSeconds(nameof(PlayBomPS), 5f);
    }

    public void PlayBomPS()
    {
        _bomPS.SetActive(true);
        DisplayFlg = false;
        RequestSerialization();
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            foreach (BurntCake_PickupMain item in _burntChipObj)
            {
                item.Show();
                item.SetFly();
            }
        }
        LightingCount = 0;
        LightingOptionCount = 0;
        HideCandleSub(_trans0);
        HideCandleSub(_trans1);
        HideCandleNum();
        SetCandleDisplay(_candleHBObj, false);
        SetCandleDisplay(_candleAnnivObj, false);
        RequestSerialization();
        SendCustomEventDelayedSeconds(nameof(StopBomPS), 3f);
    }

    public void StopBomPS() => _bomPS.SetActive(false);

    public void PlayNormalBGM()
    {
        _bgm_N.Play();
        SendCustomEventDelayedSeconds(nameof(FalseProtectionFlg), 12f);
    }

    public void FalseProtectionFlg()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(FalseProtectionFlgSub));
    }

    public void FalseProtectionFlgSub()
    {
        FalseProtectionFlgSub(_trans0);
        FalseProtectionFlgSub(_trans1);
        SetCandleProtection(_candleHBObj, false);
        SetCandleProtection(_candleAnnivObj, false);
    }

    void FalseProtectionFlgSub(Transform trans)
    {
        foreach (Transform child in trans)
        {
            CakeCandleGimmick ccg = child.GetComponent<CakeCandleGimmick>();
            if (ccg != null) SetCandleProtection(ccg, false);
        }
    }

    public void CreateCandle()
    {
        ExplosionFlg = Random.Range(0, 100) < _explosionProbability;
        CheckLightingFlg = false;
        if (CandleNormalCount == 1) ShowCandleOne(_trans0);
        else ShowCandleRing(_trans0, CandleNormalCount <= 10 ? CandleNormalCount : CandleNormalCount / 2, _r0);
        if (CandleNormalCount > 10)
        {
            int halfCount = CandleNormalCount / 2;
            if (CandleNormalCount % 2 != 0) halfCount += 1;
            ShowCandleRing(_trans1, halfCount, _r1);
        }
        if (CandleNumFlg) ShowCandleNum();
        SetCandleDisplay(_candleHBObj, CandleHappyBirthdayFlg);
        SetCandleDisplay(_candleAnnivObj, CandleAnnivFlg);
        RequestSerialization();
    }

    void ShowCandleRing(Transform parent, int count, float radius)
    {
        int subCount = parent.GetChild(0).GetComponent<CakeCandleGimmick>()._meshR[CandleType].Length;
        for (int i = 0; i < count; i++)
        {
            CakeCandleGimmick ccg = parent.GetChild(i).GetComponent<CakeCandleGimmick>();
            float angle = (360f / count) * i * Mathf.Deg2Rad;
            ccg.CandleTypeMain = CandleType;
            ccg.CandleTypeSub = Random.Range(0, subCount);
            ccg.CandlePos = new Vector3(radius * Mathf.Cos(angle), 0, radius * Mathf.Sin(angle));
            ccg.DisplayFlg = true;
            ccg.ProtectionFlg = true;
            ccg.UpdateValue();
        }
    }

    void ShowCandleOne(Transform parent)
    {
        int subCount = parent.GetChild(0).GetComponent<CakeCandleGimmick>()._meshR[CandleType].Length;
        CakeCandleGimmick ccg = parent.GetChild(0).GetComponent<CakeCandleGimmick>();
        ccg.CandleTypeMain = CandleType;
        ccg.CandleTypeSub = Random.Range(0, subCount);
        if (CandleHappyBirthdayFlg || CandleAnnivFlg) ccg.CandlePos = new Vector3(0.05f, 0, 0);
        else ccg.CandlePos = Vector3.zero;
        ccg.DisplayFlg = true;
        ccg.ProtectionFlg = true;
        ccg.UpdateValue();
    }

    void ShowCandleNum()
    {
        string formattedNumber = CandleNumCount.ToString("D6");
        string trimmedNumber = RemoveLeadingZeros(formattedNumber);

        if (trimmedNumber.Length == 0) return;

        int digitCount = trimmedNumber.Length;
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            LightingOptionCount = LightingOptionCount + digitCount;
        }
        float[] offsets = GetOffsets(digitCount);

        for (int i = 0; i < digitCount; i++)
        {
            int digit = trimmedNumber[i] - '0';

            if (_candleNumObj[i] != null)
            {
                _candleNumObj[i].CandleNo = digit;
                _candleNumObj[i].DisplayFlg = true;

                float xPos = offsets[i];
                _candleNumObj[i].CandlePos = new Vector3(xPos, 0, 0);
                _candleNumObj[i].UpdateValue();
            }
        }
    }

    float[] GetOffsets(int digitCount)
    {
        float startX = ((digitCount - 1) * _wide) / 2;

        float[] positions = new float[digitCount];
        for (int i = 0; i < digitCount; i++)
        {
            positions[i] = startX - (i * _wide);
        }
        return positions;
    }

    string RemoveLeadingZeros(string numStr)
    {
        int index = 0;
        while (index < numStr.Length && numStr[index] == '0')
        {
            index++;
        }
        return (index == numStr.Length) ? "0" : numStr.Substring(index);
    }

    public void HideCandle()
    {
        DisplayFlg = true;
        LightingCount = 0;
        LightingOptionCount = 0;
        CheckLightingFlg = true;
        HideCandleSub(_trans0);
        HideCandleSub(_trans1);
        HideCandleNum();
        SetCandleDisplay(_candleHBObj, false);
        SetCandleDisplay(_candleAnnivObj, false);
    }

    public void HideCandleNum()
    {
        for (int i = 0; i < _candleNumObj.Length; i++)
        {
            _candleNumObj[i].HideCandle();
            _candleNumObj[i].UpdateValueSub();
        }
    }

    public void HideCandleSub(Transform trans)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            CakeCandleGimmick ccg = trans.GetChild(i).GetComponent<CakeCandleGimmick>();
            ccg.DisplayFlg = false;
            ccg.CandlePos = Vector3.zero;
            ccg.FireFlg = false;
            ccg.ProtectionFlg = true;
            ccg.UpdateValue();
        }
    }

    void SetCandleDisplay(CakeCandleBase candle, bool state)
    {
        candle.DisplayFlg = state;
        candle.FireFlg = false;
        candle.ProtectionFlg = true;
        candle.UpdateValue();
        if (state)
        {
            if (Networking.LocalPlayer.IsOwner(gameObject))
            {
                ++LightingOptionCount;
            }
            RequestSerialization();
        }
    }

    void SetCandleProtection(CakeCandleBase candle, bool state)
    {
        candle.ProtectionFlg = state;
        candle.UpdateValue();
    }

    public void EatCake()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(EatCakeSub0));
    }

    public void EatCakeSub0()
    {
        if (EatCount == 8) SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(HideCandle));
        --EatCount;
        RequestSerialization();
    }

    public void UpdateValue() => SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(UpdateValueSub));
    public void UpdateValueSub() => RequestSerialization();

    public void Reset()
    {
        VRCPickup p = (VRCPickup)_sub.GetComponent(typeof(VRCPickup));
        if (p != null)
        {
            p.Drop();
        }
        _sub.transform.position = new Vector3(0, -10000f, 0);
        _sub.transform.rotation = Quaternion.identity;
        DisplayFlg = true;
        CandleNormalCount = 0;
        CandleNumCount = 0;
        CandleType = 0;
        CandleNumFlg = false;
        CandleHappyBirthdayFlg = false;
        CandleAnnivFlg = false;
        LightingCount = 0;
        LightingOptionCount = 0;
        CheckLightingFlg = false;
        ExplosionFlg = false;
        EatCount = 8;
        _bgm_N.Stop();
        _bgm_Bom.Stop();
        _bomPS.SetActive(false);
        foreach (BurntCake_PickupMain item in _burntChipObj)
        {
            item.HideSub();
        }
        HideCandleSub(_trans0);
        HideCandleSub(_trans1);
        HideCandleNum();
        SetCandleDisplay(_candleHBObj, false);
        SetCandleDisplay(_candleAnnivObj, false);
        RequestSerialization();
    }
}
