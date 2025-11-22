
using Newtonsoft.Json.Linq;
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using static UnityEngine.ParticleSystem;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class RealCharger_PickupMain : UdonSharpBehaviour
{
    public RealCharger_PickupSub _sub;
    [SerializeField] VRC_Pickup _pickup;
    [SerializeField] Transform _trans;
    [SerializeField] float _limitHeight = 0.55f;
    [SerializeField] AudioSource _chocoAS;
    [SerializeField] AudioSource _shootAS;
    [SerializeField] BoxCollider _collSub;
    [SerializeField] BoxCollider _collMain;
    [SerializeField] MeshRenderer _mr;
    [SerializeField] SkewerObj _so;
    [SerializeField] ParticleSystem[] _ps;
    bool _meshRFlg = false;
    [SerializeField] Text _debugTxt;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(PointedFood))] string _pointedFood = "";
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(PlayParticle))] int _playPS = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(HeightFlg))] bool _heightFlg = true;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ChocoFlg))] bool _chocoFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(AddChocoFlg))] bool _addChocoFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(AddChocoValue))] float _addChocoValue = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(AddMintFlg))] bool _addMintFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(AddMintValue))] float _addMintValue = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(AddPinkFlg))] bool _addPinkFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(AddPinkValue))] float _addPinkValue = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(AddWhiteFlg))] bool _addWhiteFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(AddWhiteValue))] float _addWhiteValue = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(PickupFlg))] bool _pickupFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ResetCount))] int _resetCount = 0;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(DebugTxt))] string _debugStr = "";

    public string PointedFood
    {
        get => _pointedFood;
        set
        {
            string[] splitText0 = _pointedFood.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            float sumHight = 0;
            string updatePointedFood = "";
            //掛け
            if (value == "Choco" || value == "Mint"|| value == "Pink" || value == "White")
            {
                if (0 < splitText0.Length) ChocoFlg = true;
                DeleteFoodObj();
                for (int i = 0; i < splitText0.Length; i++)
                {
                    string[] splitText1 = splitText0[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    int typeInt = int.Parse(splitText1[0]);
                    float heightFloat = float.Parse(splitText1[1]);
                    sumHight += heightFloat;
                    GameObject tmpObj = default;
                    //種類分け
                    int divInt = typeInt / 5;
                    if (value == "Choco")
                    {
                        int chocoType = divInt * 5 + 1;
                        tmpObj = CreateFoodObj(chocoType);
                        updatePointedFood += $"_{chocoType},{heightFloat}";
                    }
                    else if (value == "Mint")
                    {
                        int chocoType = divInt * 5 + 2;
                        tmpObj = CreateFoodObj(chocoType);
                        updatePointedFood += $"_{chocoType},{heightFloat}";
                    }
                    else if (value == "Pink")
                    {
                        int chocoType = divInt * 5 + 3;
                        tmpObj = CreateFoodObj(chocoType);
                        updatePointedFood += $"_{chocoType},{heightFloat}";
                    }
                    else if (value == "White")
                    {
                        int chocoType = divInt * 5 + 4;
                        tmpObj = CreateFoodObj(chocoType);
                        updatePointedFood += $"_{chocoType},{heightFloat}";
                    }
                    tmpObj.transform.parent = _trans;
                    tmpObj.transform.SetSiblingIndex(i);
                    tmpObj.transform.localPosition = new Vector3(0, 0, -sumHight);
                    tmpObj.transform.localRotation = Quaternion.identity;
                }
                _pointedFood = updatePointedFood;
            }
            else
            {
                _pointedFood = $"{value}";
                DeleteFoodObj();
                string[] splitValue = value.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                //刺してあるFoodの位置ずらし
                for (int i = 0; i < splitValue.Length; i++)
                {
                    string[] splitText1 = splitValue[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    int typeInt = int.Parse(splitText1[0]);
                    float heightFloat = float.Parse(splitText1[1]);
                    sumHight += heightFloat;
                    GameObject tmpObj1 = CreateFoodObj(typeInt);
                    tmpObj1.transform.parent = _trans;
                    tmpObj1.transform.localPosition = new Vector3(0, 0, -sumHight);
                    tmpObj1.transform.localRotation = Quaternion.identity;
                }
                if (_limitHeight <= sumHight) HeightFlg = false;
                else HeightFlg = true;
                if (splitValue.Length == 0) ChocoFlg = false;
            }
        }
    }

    public GameObject CreateFoodObj(int no)
    {
        if (_so._obj.Length <= no) return Instantiate(_so._obj[0]);
        return Instantiate(_so._obj[no]);
    }

    public void DeleteFoodObj()
    {
        for (int i = _trans.childCount - 1; i >= 0; i--)
        {
            Destroy(_trans.GetChild(i).gameObject);
        }
    }

    public int PlayParticle
    {
        get => _playPS;
        set
        {
            _playPS = value;
            if (Networking.LocalPlayer.IsOwner(gameObject)) SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlayParticleSub));
        }
    }

    public void PlayParticleSub()
    {
        if (_ps.Length <= PlayParticle) return;
        _ps[PlayParticle].Play();
        _shootAS.Play();
    }

    public bool HeightFlg
    {
        get => _heightFlg;
        set
        {
            _heightFlg = value;
        }
    }

    public bool ChocoFlg
    {
        get => _chocoFlg;
        set
        {
            _chocoFlg = value;
        }
    }

    public bool AddChocoFlg
    {
        get => _addChocoFlg;
        set
        {
            _addChocoFlg = value;
            if (_addChocoFlg) _chocoAS.Play();
            else _chocoAS.Stop();
        }
    }

    public float AddChocoValue
    {
        get => _addChocoValue;
        set
        {
            _addChocoValue = value;
            if (!ChocoFlg && 1.5 < _addChocoValue)
            {
                PointedFood = "Choco";
                _chocoAS.Stop();
            }
        }
    }

    public bool AddMintFlg
    {
        get => _addMintFlg;
        set
        {
            _addMintFlg = value;
            if (_addMintFlg) _chocoAS.Play();
            else _chocoAS.Stop();
        }
    }

    public float AddMintValue
    {
        get => _addMintValue;
        set
        {
            _addMintValue = value;
            if (!ChocoFlg && 1.5 < _addMintValue)
            {
                PointedFood = "Mint";
                _chocoAS.Stop();
            }
        }
    }

    public bool AddPinkFlg
    {
        get => _addPinkFlg;
        set
        {
            _addPinkFlg = value;
            if (_addPinkFlg) _chocoAS.Play();
            else _chocoAS.Stop();
        }
    }

    public float AddPinkValue
    {
        get => _addPinkValue;
        set
        {
            _addPinkValue = value;
            if (!ChocoFlg && 1.5 < _addPinkValue)
            {
                PointedFood = "Pink";
                _chocoAS.Stop();
            }
        }
    }

    public bool AddWhiteFlg
    {
        get => _addWhiteFlg;
        set
        {
            _addWhiteFlg = value;
            if (_addWhiteFlg) _chocoAS.Play();
            else _chocoAS.Stop();
        }
    }

    public float AddWhiteValue
    {
        get => _addWhiteValue;
        set
        {
            _addWhiteValue = value;
            if (!ChocoFlg && 1.5 < _addWhiteValue)
            {
                PointedFood = "White";
                _chocoAS.Stop();
            }
        }
    }

    public bool PickupFlg
    {
        get => _pickupFlg;
        set
        {
            _pickupFlg = value;
        }
    }

    public int ResetCount
    {
        get => _resetCount;
        set
        {
            _resetCount = value;
            if (3 <= _resetCount)
            {
                ResetSub();
                _resetCount = 0;
            }
        }
    }

    public bool MeshRFlg
    {
        get => _meshRFlg;
        set
        {
            _meshRFlg = value;
            _mr.enabled = _meshRFlg;
            _collSub.enabled = _meshRFlg;
            _collMain.enabled = _meshRFlg;
        }
    }

    public string DebugTxt
    {
        get => _debugStr;
        set
        {
            _debugStr = value;
            if (_debugTxt) _debugTxt.text = _debugStr;
        }
    }

    void Update()
    {
        //VRCPlayerApi owner = Networking.GetOwner(gameObject);
        //VRCPlayerApi owner1 = Networking.GetOwner(_sub.gameObject);
        //DebugTxt = $"PointedFood:{PointedFood}\ndisplayNameSub{owner1.displayName}\ndisplayName{owner.displayName}";
        if (!ChocoFlg && AddChocoFlg)
        {
            AddChocoValue += Time.deltaTime;
        }
        else
        {
            if (0 < AddChocoValue) AddChocoValue -= Time.deltaTime;
        }
        if (!ChocoFlg && AddMintFlg)
        {
            AddMintValue += Time.deltaTime;
        }
        else
        {
            if (0 < AddMintValue) AddMintValue -= Time.deltaTime;
        }
        if (!ChocoFlg && AddPinkFlg)
        {
            AddPinkValue += Time.deltaTime;
        }
        else
        {
            if (0 < AddPinkValue) AddPinkValue -= Time.deltaTime;
        }
        if (!ChocoFlg && AddWhiteFlg)
        {
            AddWhiteValue += Time.deltaTime;
        }
        else
        {
            if (0 < AddWhiteValue) AddWhiteValue -= Time.deltaTime;
        }
    }

    public void MainPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        PickupFlg = true;
    }

    public void MainDrop()
    {

    }

    public void MainPickupUseDown()
    {
        Debug.Log($"MainPickupUseDown    MainPickupUseDown");
        if (!ChocoFlg) return;
        string[] splitText0 = PointedFood.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
        if (0 < splitText0.Length)
        {
            string[] splitText1 = splitText0[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int typeInt = int.Parse(splitText1[0]);
            PlayParticle = typeInt;
            RequestSerialization();
            string t = "";
            if (1 < splitText0.Length) t = RemoveBeforeAndIncludingSecondUnderscore(PointedFood);
            else t = "";
            PointedFood = t;
            RequestSerialization();
        }
    }

    static string RemoveBeforeAndIncludingSecondUnderscore(string str)
    {
        int firstUnderscore = str.IndexOf('_'); // 最初のアンダーバーの位置
        if (firstUnderscore == -1) return str; // アンダーバーがない場合はそのまま返す

        int secondUnderscore = str.IndexOf('_', firstUnderscore + 1); // 2つ目のアンダーバーの位置
        if (secondUnderscore == -1) return str; // 2つ目のアンダーバーがない場合もそのまま返す

        return str.Substring(secondUnderscore);
    }

    public void FuncMeshR_ON()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(FuncMeshR_ONSub));
    }

    public void FuncMeshR_ONSub() { MeshRFlg = true; }

    public void FuncMeshR_OFF()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(FuncMeshR_OFFSub));
    }

    public void FuncMeshR_OFFSub() { MeshRFlg = false; }

    void OnTriggerEnter(Collider coll)
    {
        ChocolateFountain_PickupMain cfpm = coll.GetComponent<ChocolateFountain_PickupMain>();
        if (cfpm != null)
        {
            if (HeightFlg && !ChocoFlg)
            {
                if (Networking.LocalPlayer.IsOwner(gameObject))
                {
                    PointedFood = $"_{cfpm._type},{cfpm._height}{PointedFood}";
                    Debug.Log($"OnTriggerEnter:_{cfpm._type},{cfpm._height}");
                    cfpm.Reset();
                    RequestSerialization();
                }
            }
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            if (MeshRFlg) FuncMeshR_ON();
            else FuncMeshR_OFF();
        }
    }

    public void Reset()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ResetSub));
    }

    public void ResetSub()
    {
        VRCPickup pickup1 = (VRCPickup)_sub.gameObject.GetComponent(typeof(VRCPickup));
        if (pickup1 != null)
        {
            pickup1.Drop();
        }
        FuncMeshR_OFF();
        ResetCount = 0;
        PickupFlg = false;
        PointedFood = "";
        AddChocoFlg = false;
        AddChocoValue = 0;
        AddMintFlg = false;
        AddMintValue = 0;
        AddPinkFlg = false;
        AddPinkValue = 0;
        AddWhiteFlg = false;
        AddWhiteValue = 0;
        _sub.gameObject.transform.localPosition = Vector3.zero;
        _sub.gameObject.transform.localRotation = Quaternion.identity;
        RequestSerialization();
    }
}
