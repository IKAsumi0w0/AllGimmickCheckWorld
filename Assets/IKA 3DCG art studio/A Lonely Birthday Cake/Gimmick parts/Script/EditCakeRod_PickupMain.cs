
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class EditCakeRod_PickupMain : UdonSharpBehaviour
{
    public EditCakeRod_PickupSub _sub;
    public WholeCake_PickupMain _wcp;
    [SerializeField] GameObject _uiObj;
    [SerializeField] LineRenderer _lineR;
    [SerializeField] AudioSource _hitSE;
    [SerializeField] Button _applyBtn;
    [SerializeField] BtnController _candleType0Btn;
    [SerializeField] BtnController _candleType1Btn;
    [SerializeField] BtnController _candleTypeNumBtn;
    [SerializeField] BtnController _candleTypeHBBtn;
    [SerializeField] BtnController _candleTypeAnnivBtn;

    public Text[] digitNormalTexts;
    public Text[] digitNumTexts;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(OpenFlg))] bool _openFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CandleNormalCount))] int _candleNormalCount = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CandleNumCount))] int _candleNumCount = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CandleType))] int _candleType = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CandleNumFlg))] bool _candleNumFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CandleHBFlg))] bool _candleHBFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(CandleAnnivFlg))] bool _candleAnnivFlg = false;

    public bool OpenFlg
    {
        get => _openFlg;
        set
        {
            _openFlg = value;
            _uiObj.SetActive(_openFlg);
        }
    }

    public int CandleNormalCount
    {
        get => _candleNormalCount;
        set
        {
            _candleNormalCount = value;
            string formattedNumber = _candleNormalCount.ToString("D2");
            for (int i = 0; i < digitNormalTexts.Length; i++)
            {
                int placeValue = (int)Mathf.Pow(10, i);
                digitNormalTexts[i].text = ((CandleNormalCount / placeValue) % 10).ToString();
            }
        }
    }

    public int CandleNumCount
    {
        get => _candleNumCount;
        set
        {
            _candleNumCount = value;
            string formattedNumber = _candleNumCount.ToString("D6");
            for (int i = 0; i < digitNumTexts.Length; i++)
            {
                int placeValue = (int)Mathf.Pow(10, i);
                digitNumTexts[i].text = ((CandleNumCount / placeValue) % 10).ToString();
            }
        }
    }

    public int CandleType
    {
        get => _candleType;
        set
        {
            _candleType = value;
            if (value == 0 || value == 1)
            {
                _candleTypeNumBtn.SelectFlg = false;
                if (value == 0)
                {
                    _candleType0Btn.SelectFlg = true;
                    _candleType1Btn.SelectFlg = false;
                }
                else if (value == 1)
                {
                    _candleType0Btn.SelectFlg = false;
                    _candleType1Btn.SelectFlg = true;
                }
            }
        }
    }

    public bool CandleNumFlg
    {
        get => _candleNumFlg;
        set
        {
            _candleNumFlg = value;
            _candleTypeNumBtn.SelectFlg = _candleNumFlg;
        }
    }

    public bool CandleHBFlg
    {
        get => _candleHBFlg;
        set
        {
            _candleHBFlg = value;
            _candleTypeHBBtn.SelectFlg = _candleHBFlg;
            if (_candleHBFlg && CandleAnnivFlg)
            {
                CandleAnnivFlg = false;
                _candleTypeAnnivBtn.SelectFlg = false;
            }
        }
    }

    public bool CandleAnnivFlg
    {
        get => _candleAnnivFlg;
        set
        {
            _candleAnnivFlg = value;
            _candleTypeAnnivBtn.SelectFlg = _candleAnnivFlg;
            if (_candleAnnivFlg && CandleHBFlg)
            {
                CandleHBFlg = false;
                _candleTypeHBBtn.SelectFlg = false;
            }
        }
    }

    private void Start()
    {
        if (_lineR != null)
        {
            _lineR.startWidth = 0.01f;
            _lineR.endWidth = 0.01f;
        }
    }

    // 各桁の増加処理
    public void IncreaseNumDigit0() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(IncreaseNumDigit0Sub)); }
    public void IncreaseNumDigit1() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(IncreaseNumDigit1Sub)); }
    public void IncreaseNumDigit2() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(IncreaseNumDigit2Sub)); }
    public void IncreaseNumDigit3() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(IncreaseNumDigit3Sub)); }

    public void IncreaseNumDigit0Sub() { ChangeNumDigit(0, 1); }
    public void IncreaseNumDigit1Sub() { ChangeNumDigit(1, 1); }
    public void IncreaseNumDigit2Sub() { ChangeNumDigit(2, 1); }
    public void IncreaseNumDigit3Sub() { ChangeNumDigit(3, 1); }

    // 各桁の減少処理
    public void DecreaseNumDigit0() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(DecreaseNumDigit0Sub)); }
    public void DecreaseNumDigit1() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(DecreaseNumDigit1Sub)); }
    public void DecreaseNumDigit2() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(DecreaseNumDigit2Sub)); }
    public void DecreaseNumDigit3() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(DecreaseNumDigit3Sub)); }

    public void DecreaseNumDigit0Sub() { ChangeNumDigit(0, -1); }
    public void DecreaseNumDigit1Sub() { ChangeNumDigit(1, -1); }
    public void DecreaseNumDigit2Sub() { ChangeNumDigit(2, -1); }
    public void DecreaseNumDigit3Sub() { ChangeNumDigit(3, -1); }

    public void ChangeNumDigit(int digitIndex, int change)
    {
        int placeValue = (int)Mathf.Pow(10, digitIndex);
        int digitValue = (CandleNumCount / placeValue) % 10;
        int newDigitValue = digitValue + change;

        if (newDigitValue < 0) newDigitValue = 9;
        if (newDigitValue > 9) newDigitValue = 0;

        int difference = (newDigitValue - digitValue) * placeValue;
        int newNumber = CandleNumCount + difference;

        if (newNumber < 0) newNumber = 9999;
        if (newNumber > 9999) newNumber = 0;

        CandleNumCount = newNumber;
        RequestSerialization();
    }

    // 各桁の増加処理
    public void IncreaseNormalDigit0() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(IncreaseNormalDigit0Sub)); }
    public void IncreaseNormalDigit1() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(IncreaseNormalDigit1Sub)); }

    public void IncreaseNormalDigit0Sub() { ChangeNormalDigit(0, 1); }
    public void IncreaseNormalDigit1Sub() { ChangeNormalDigit(1, 1); }

    // 各桁の減少処理
    public void DecreaseNormalDigit0() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(DecreaseNormalDigit0Sub)); }
    public void DecreaseNormalDigit1() { SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(DecreaseNormalDigit1Sub)); }

    public void DecreaseNormalDigit0Sub() { ChangeNormalDigit(0, -1); }
    public void DecreaseNormalDigit1Sub() { ChangeNormalDigit(1, -1); }

    public void ChangeNormalDigit(int digitIndex, int change)
    {
        int placeValue = (int)Mathf.Pow(10, digitIndex);
        int digitValue = (CandleNormalCount / placeValue) % 10; // 対象桁の値を取得
        int newDigitValue = digitValue + change;
        if (newDigitValue < 0) newDigitValue = 9;
        if (newDigitValue > 9) newDigitValue = 0;

        int difference = (newDigitValue - digitValue) * placeValue; // 差分を計算
        int newNumber = CandleNormalCount + difference;

        if (newNumber < 0) newNumber = 99;
        if (newNumber > 99) newNumber = 0;

        CandleNormalCount = newNumber;
        RequestSerialization();
    }

    void Update()
    {
        if (OpenFlg && _wcp != null)
        {
            Vector3[] points = new Vector3[] { _wcp.transform.position, _lineR.transform.position };
            _lineR.SetPositions(points);
            _lineR.positionCount = 2;
        }
        else if (!OpenFlg || _wcp ==  null)
        {
            _lineR.positionCount = 0;
        }
    }

    void OnTriggerStay(Collider coll)
    {
        WholeCake_PickupMain wcp = coll.GetComponent<WholeCake_PickupMain>();
        if (wcp != null && _wcp == null)
        {
            _wcp = wcp;
            if (Networking.LocalPlayer.IsOwner(gameObject)) SetCakeOwner();
            _hitSE.Play();
        }
        else if (wcp != null && wcp != _wcp)
        {
            _wcp = null;
        }
    }

    public void SetCakeOwner()
    {
        if (!Networking.LocalPlayer.IsOwner(_wcp.gameObject)) Networking.SetOwner(Networking.LocalPlayer, _wcp.gameObject);
        foreach (Transform item in _wcp._trans0)
        {
            if (!Networking.LocalPlayer.IsOwner(item.gameObject)) Networking.SetOwner(Networking.LocalPlayer, item.gameObject);
        }
        foreach (Transform item in _wcp._trans1)
        {
            if (!Networking.LocalPlayer.IsOwner(item.gameObject)) Networking.SetOwner(Networking.LocalPlayer, item.gameObject);
        }
        foreach (CakeNumCandleGimmick item in _wcp._candleNumObj)
        {
            if (!Networking.LocalPlayer.IsOwner(item.gameObject)) Networking.SetOwner(Networking.LocalPlayer, item.gameObject);
        }
        if (!Networking.LocalPlayer.IsOwner(_wcp._candleHBObj.gameObject)) Networking.SetOwner(Networking.LocalPlayer, _wcp._candleHBObj.gameObject);
        if (!Networking.LocalPlayer.IsOwner(_wcp._candleAnnivObj.gameObject)) Networking.SetOwner(Networking.LocalPlayer, _wcp._candleAnnivObj.gameObject);

    }

    public void MainPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (_wcp != null) SetCakeOwner();
    }

    public void MainDrop()
    {
        if (OpenFlg)
        {
            Vector3 rot = _sub.transform.rotation.eulerAngles;
            rot.x = 0;
            rot.z = 0;
            _sub.transform.rotation = Quaternion.Euler(rot);
        }
    }

    public void MainPickupUseDown()
    {
        if (_wcp != null)
        {
            OpenFlg = !OpenFlg;
            RequestSerialization();
        }
    }

    public void FuncApply()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(FuncApplySub0));
    }

    public void FuncApplySub0()
    {
        if (_wcp != null)
        {
            _wcp.CandleNormalCount = CandleNormalCount;
            _wcp.CandleNumCount = CandleNumCount;
            _wcp.CandleType = CandleType;
            _wcp.CandleNumFlg = CandleNumFlg;
            _wcp.CandleHappyBirthdayFlg = CandleHBFlg;
            _wcp.CandleAnnivFlg = CandleAnnivFlg;
            _wcp.HideCandle();
            _wcp.UpdateValue();
            RequestSerialization();
        }
        SendCustomEventDelayedSeconds(nameof(FuncApplySub1), 1f, VRC.Udon.Common.Enums.EventTiming.Update);
    }

    public void FuncApplySub1()
    {

        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(FuncApplySub2));
    }

    public void FuncApplySub2()
    {
        if (_wcp != null)
        {
            _wcp.CreateCandle();
            RequestSerialization();
        }
    }

    public void CandleType0()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(CandleType0Sub));
    }

    public void CandleType0Sub()
    {
        CandleType = 0;
        RequestSerialization();
    }

    public void CandleType1()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(CandleType1Sub));
    }

    public void CandleType1Sub()
    {
        CandleType = 1;
        RequestSerialization();
    }

    public void CandleTypeNum()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(CandleNumSub));
    }

    public void CandleNumSub()
    {
        CandleNumFlg = !CandleNumFlg;
        RequestSerialization();
    }

    public void CandleHB()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(CandleHBSub));
    }

    public void CandleHBSub()
    {
        CandleHBFlg = !CandleHBFlg;
        RequestSerialization();
    }

    public void CandleAnniv()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(CandleAnnivSub));
    }

    public void CandleAnnivSub()
    {
        CandleAnnivFlg = !CandleAnnivFlg;
        RequestSerialization();
    }

    public void Reset()
    {
        VRCPickup p = (VRCPickup)_sub.GetComponent(typeof(VRCPickup));
        if (p != null)
        {
            p.Drop();
        }
        _sub.transform.position = new Vector3(0, -10000f, 0);
        _sub.transform.rotation = Quaternion.identity;
        OpenFlg = false;
        CandleNormalCount = 0;
        CandleNumCount = 0;
        CandleType = 0;
        CandleNumFlg = false;
        CandleHBFlg = false;
        CandleAnnivFlg = false;
        RequestSerialization();
    }
}
