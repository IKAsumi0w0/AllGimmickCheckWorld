
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class HalloweenTeaParty_Gimmick : UdonSharpBehaviour
{
    [SerializeField] SkinnedMeshRenderer _skinMeshR;
    [SerializeField] GameObject _psObj;
    [SerializeField] HalloweenTeaParty_Coll _htpc;
    // 任意フレームごとに処理
    [SerializeField] int _updateInterval = 3;
    // たまに1フレーム増やす確率（0〜1）
    [SerializeField] float _jitterChance = 0.1f;
    int _frameCounter;
    int _jitterOffset;
    float _checkFloat = 0;
    bool _pickupUseFlg = false;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ShapekeyFloat))] float _shapekeyFloat = 100;

    public float ShapekeyFloat
    {
        get => _shapekeyFloat;
        set
        {
            _shapekeyFloat = value;
            if (_shapekeyFloat < 0) _shapekeyFloat = 0;
            else if (100 < _shapekeyFloat) _shapekeyFloat = 100;
            if (_shapekeyFloat < 80) _psObj.SetActive(true);
            else _psObj.SetActive(false);
            _skinMeshR.SetBlendShapeWeight(0, _shapekeyFloat);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            if (_pickupUseFlg)
            {
                ShapekeyFloat += 0.5f;
            }
            _frameCounter++;
            // 指定間隔 + ジッター に一致したときだけ処理
            if (_frameCounter % (_updateInterval + _jitterOffset) == 0)
            {
                // 1フレーム増やす処理
                _jitterOffset = Random.value < _jitterChance ? 1 : 0;
                if (_checkFloat != ShapekeyFloat)
                {
                    _checkFloat = ShapekeyFloat;
                    RequestSerialization();
                }
            }
        }
    }

    public void MainPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        Networking.SetOwner(Networking.LocalPlayer, _htpc.gameObject);
    }

    public void MainDrop()
    {
        _pickupUseFlg = false;
    }

    public void MainPickupUseDown()
    {
        _pickupUseFlg = true;
    }

    public void MainPickupUseUp()
    {
        _pickupUseFlg = false;
    }

}
