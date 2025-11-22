using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using Random = UnityEngine.Random;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ShapedRiceBall_Gimmick : UdonSharpBehaviour
{
    public ShapedRiceBall_Pickup _sub;
    public RiceSet_Manager _setM;
    public int _explosionProbability = 0;
    public int _rollProbability = 0;
    [SerializeField] float forceStrength = 3f; // 力の強さを調整するための変数
    [SerializeField] Collider _collSub;
    [SerializeField] Collider _collMain;
    [SerializeField] Rigidbody _rigi;
    [SerializeField] VRCPickup _vRCPickup;
    [SerializeField] MeshRenderer _mr;
    [SerializeField] ParentConstraint _parentConstraint;
    [SerializeField] GameObject _onigiriOrigin;
    [SerializeField] GameObject _onigiriOriginGreenOnion;
    [SerializeField] GameObject _onigiriNormal;
    [SerializeField] GameObject _onigiriUme;
    [SerializeField] GameObject _onigiriGreenOnion;
    [SerializeField] GameObject _onigiriNormalNori;
    [SerializeField] GameObject _onigiriUmeNori;
    [SerializeField] GameObject _onigiriGreenOnionNori;
    [SerializeField] GameObject _onigiriMentaikoNori;
    [SerializeField] GameObject _onigiriOriginMentai;
    [SerializeField] GameObject _onigiriMentaiNasi;
    [SerializeField] SkinnedMeshRenderer[] _skinMeshR;
    [SerializeField] GameObject _psExplosion;
    [SerializeField] Animator _anime;
    [SerializeField] AudioSource _asEat;
    [SerializeField] AudioSource _asCute;
    [SerializeField] float _rollHorizon = 0.3f;
    [SerializeField] float _rollVertical = 0.5f;
    bool _pickupUseFlg = false;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(MeshRFlg))] bool _meshRFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(TypeNo))] public int _typeNo = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(GripCount))] int _gripCount = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ShapekeyInt))] int _shapekeyInt = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(PaddleNo))] int _paddleNo = -1;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(PlateNo))] int _plateNo = -1;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(OniNo))] int _oniNo = -1;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(RollFlg))] bool _rollFlg = false;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(PosSync))] Vector3 _posSync = Vector3.zero;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(RotSync))] Quaternion _rotSync = Quaternion.identity;

    public bool MeshRFlg
    {
        get => _meshRFlg;
        set
        {
            _meshRFlg = value;
            _mr.enabled = _meshRFlg;
            _collSub.enabled = _meshRFlg;
        }
    }

    public int TypeNo
    {
        get => _typeNo;
        set
        {
            _typeNo = value;
            _onigiriOrigin.SetActive(false);
            _onigiriOriginGreenOnion.SetActive(false);
            _onigiriNormal.SetActive(false);
            _onigiriUme.SetActive(false);
            _onigiriGreenOnion.SetActive(false);
            _onigiriNormalNori.SetActive(false);
            _onigiriUmeNori.SetActive(false);
            _onigiriGreenOnionNori.SetActive(false);
            _onigiriMentaikoNori.SetActive(false);
            _onigiriOriginMentai.SetActive(false);
            _onigiriMentaiNasi.SetActive(false);
            switch (_typeNo)
            {
                case 0: _onigiriOrigin.SetActive(true); break;
                case 1: _onigiriOriginGreenOnion.SetActive(true); break;
                case 2: _onigiriNormal.SetActive(true); break;
                case 3: _onigiriGreenOnion.SetActive(true); break;
                case 4: _onigiriUme.SetActive(true); break;
                case 5: _onigiriNormalNori.SetActive(true); break;
                case 6: _onigiriGreenOnionNori.SetActive(true); break;
                case 7: _onigiriUmeNori.SetActive(true); break;
                case 8: _onigiriMentaikoNori.SetActive(true); break;
                case 9: _onigiriOriginMentai.SetActive(true); break;
                case 10: _onigiriMentaiNasi.SetActive(true); break;
                default: _onigiriOrigin.SetActive(true); break;
            }
            if (_typeNo != 0) _asEat.Play();
        }
    }

    public int GripCount
    {
        get => _gripCount;
        set
        {
            _gripCount = value;
            if (3 <= _gripCount)
            {
                if (TypeNo == 0) TypeNo = 2;
                if (TypeNo == 1) TypeNo = 3;
                if (TypeNo == 9) TypeNo = 10;
                _gripCount = 0;
            }
            if (_gripCount != 0 && _gripCount <= 2)
            {
                _anime.SetTrigger("hold_t");
                _asEat.Play();
            }
        }
    }

    public int ShapekeyInt
    {
        get => _shapekeyInt;
        set
        {
            _shapekeyInt = value;
            for (int i = 0; i < _skinMeshR.Length; i++)
                _skinMeshR[i].SetBlendShapeWeight(0, _shapekeyInt);

            if (Networking.LocalPlayer.IsOwner(gameObject) && 100 < _shapekeyInt) Reset();
            if (_shapekeyInt != 0) _asEat.Play();
        }
    }

    public int PlateNo
    {
        get => _plateNo;
        set
        {
            _plateNo = value;

            if (_paddleNo == -1 && _plateNo == -1)
            {
                UnsetParentConstraint();
            }
            else if (_plateNo != -1)
            {
                
            }

            if (_plateNo != -1) _asCute.Play();
        }
    }

    public int PaddleNo
    {
        get => _paddleNo;
        set
        {
            _paddleNo = value;
            if (_paddleNo == -1 && PlateNo == -1)
            {
                UnsetParentConstraint();
            }
            else if (_paddleNo != -1)
            {
                for (int i = _parentConstraint.sourceCount - 1; i >= 0; --i)
                    _parentConstraint.RemoveSource(i);

                ConstraintSource source = new ConstraintSource();
                source.sourceTransform = _setM._rpm._objs[_paddleNo]._main._setPosTrans;
                source.weight = 1.0f;
                int index = _parentConstraint.AddSource(source);

                _parentConstraint.SetTranslationOffset(index, Vector3.zero);
                _parentConstraint.SetRotationOffset(index, Vector3.zero);
                _parentConstraint.transform.position = _setM._rpm._objs[_paddleNo]._main._setPosTrans.position;
                _parentConstraint.transform.rotation = _setM._rpm._objs[_paddleNo]._main._setPosTrans.rotation;
                _parentConstraint.constraintActive = true;
                _asCute.Play();
            }
        }
    }

    public int OniNo
    {
        get => _oniNo;
        set { _oniNo = value; }
    }

    public bool RollFlg
    {
        get => _rollFlg;
        set
        {
            _rollFlg = value;
            _rigi.useGravity = value;
            _rigi.isKinematic = !value;
            _collSub.isTrigger = !value;
        }
    }

    public Vector3 PosSync
    {
        get => _posSync;
        set
        {
            _posSync = value;
            if (!Networking.LocalPlayer.IsOwner(gameObject) && _posSync != Vector3.zero && RotSync != Quaternion.identity) SetConstraintSource();
        }
    }

    public Quaternion RotSync
    {
        get => _rotSync;
        set
        {
            _rotSync = value;
            if (!Networking.LocalPlayer.IsOwner(gameObject) && PosSync != Vector3.zero && _rotSync != Quaternion.identity) SetConstraintSource();
        }
    }

    void Start()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
            TypeNo = 0;
    }

    public void MainPickup()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
        _rigi.velocity = Vector3.zero;
        _rigi.angularVelocity = Vector3.zero;
        MeshRFlg = true;
        PosSync = Vector3.zero;
        RotSync = Quaternion.identity;
        RequestSerialization();
        if (PaddleNo != -1)
            _setM._rpm._objs[PaddleNo]._main.RicePaddleToReset();

        PaddleNo = -1;
        PlateNo = -1;
        RequestSerialization();
        _pickupUseFlg = false;
    }

    public void MainDrop()
    {
        if (PlateNo != -1)
        {
            if (_setM == null || _setM._bpm == null || _setM._bpm._objs == null) return;
            if (PlateNo < 0 || PlateNo >= _setM._bpm._objs.Length) return;

            var plate = _setM._bpm._objs[PlateNo];
            if (plate == null || plate._main == null || plate._main._setPosTrans == null) return;

            for (int i = _parentConstraint.sourceCount - 1; i >= 0; --i)
                _parentConstraint.RemoveSource(i);

            ConstraintSource source = new ConstraintSource();
            source.sourceTransform = _setM._bpm._objs[PlateNo]._main._setPosTrans;
            source.weight = 1f;
            int index = _parentConstraint.AddSource(source);

            // 現在見た目維持オフセット
            PosSync = source.sourceTransform.InverseTransformPoint(_parentConstraint.transform.position);
            RotSync = Quaternion.Inverse(source.sourceTransform.rotation) * _parentConstraint.transform.rotation;
            Vector3 rotationOffset = RotSync.eulerAngles;

            _parentConstraint.SetTranslationOffset(index, PosSync);
            _parentConstraint.SetRotationOffset(index, rotationOffset);
            _parentConstraint.constraintActive = true;
            RequestSerialization();
        }
        else
        {
            PosSync = Vector3.zero;
            RotSync = Quaternion.identity;
            RequestSerialization();
        }
        RequestSerialization();
    }

    public void SetConstraintSource()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject))
        {
            if (_setM == null || _setM._bpm == null || _setM._bpm._objs == null) return;
            if (PlateNo < 0 || PlateNo >= _setM._bpm._objs.Length) return;

            var plate = _setM._bpm._objs[PlateNo];
            if (plate == null || plate._main == null || plate._main._setPosTrans == null) return;

            for (int i = _parentConstraint.sourceCount - 1; i >= 0; --i)
                _parentConstraint.RemoveSource(i);

            ConstraintSource source = new ConstraintSource();
            source.sourceTransform = _setM._bpm._objs[PlateNo]._main._setPosTrans;
            source.weight = 1f;
            int index = _parentConstraint.AddSource(source);

            Vector3 rotationOffset = RotSync.eulerAngles;

            _parentConstraint.SetTranslationOffset(index, PosSync);
            _parentConstraint.SetRotationOffset(index, rotationOffset);
            _parentConstraint.constraintActive = true;

            Debug.Log(
                "[ShapedRiceBall_Gimmick] SetConstraintSource | " +
                "PlateNo=" + PlateNo +
                " PosSync=" + PosSync.ToString("F3") +
                " RotSync(Euler)=" + RotSync.eulerAngles.ToString("F1")
            );
        }
    }

    public void MainPickupUseDown()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            if (TypeNo == 0 || TypeNo == 1 || TypeNo == 9)
            {
                if (GripCount <= 3)
                {
                    ++GripCount;
                    RequestSerialization();
                }
            }
            else if (TypeNo == 5 || TypeNo == 6 || TypeNo == 7 || TypeNo == 8)
            {
                if (ShapekeyInt == 0)
                {
                    int randomValue = Random.Range(0, 100);
                    if (randomValue < _explosionProbability)
                    {
                        FuncExplosion();
                        return;
                    }

                    randomValue = Random.Range(0, 100);
                    if (randomValue < _rollProbability)
                    {
                        RollFlg = true;
                        RequestSerialization();
                        FuncRoll();
                        return;
                    }
                }
                ShapekeyInt += 100;
                RequestSerialization();
            }
            _pickupUseFlg = true;
        }
    }

    public void FuncExplosion()
    {
        _vRCPickup.Drop();

        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(RunExplosion));
        TypeNo = 0;
        MeshRFlg = false;
        SendCustomEventDelayedSeconds(nameof(Reset), 2f, VRC.Udon.Common.Enums.EventTiming.Update);
    }

    public void RunExplosion()
    {
        _psExplosion.SetActive(true);
    }

    public void MainPickupUseUp()
    {
        _pickupUseFlg = false;
    }

    void OnTriggerEnter(Collider coll)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            Umebosshi_Gimmick ume = coll.GetComponent<Umebosshi_Gimmick>();
            if (ume != null)
            {
                if (TypeNo == 2)
                {
                    TypeNo = 4;
                    ume.FuncReset();
                    RequestSerialization();
                }
            }

            Nori_Pickup nori = coll.GetComponent<Nori_Pickup>();
            if (nori != null)
            {
                if (TypeNo == 2) { TypeNo = 5; nori._main.FuncReset(); }
                else if (TypeNo == 3) { TypeNo = 6; nori._main.FuncReset(); }
                else if (TypeNo == 4) { TypeNo = 7; nori._main.FuncReset(); }
                else if (TypeNo == 10) { TypeNo = 8; nori._main.FuncReset(); }
                RequestSerialization();
            }

            Greenonioncondimentspoon negi = coll.GetComponent<Greenonioncondimentspoon>();
            if (negi != null)
            {
                if (negi.NegiMeshRFlg)
                {
                    if (TypeNo == 0) { TypeNo = 1; negi.HideNegi(); }
                    else if (TypeNo == 2) { TypeNo = 3; negi.HideNegi(); }
                }
                RequestSerialization();
            }

            Mentaiko_Pickup mentaiko = coll.GetComponent<Mentaiko_Pickup>();
            if (mentaiko != null)
            {
                if (TypeNo == 0) { TypeNo = 9; mentaiko._main.FuncReset(); }
                else if (TypeNo == 2) { TypeNo = 10; mentaiko._main.FuncReset(); }
                RequestSerialization();
            }

            BambooLeafPlate_Gimmick bpp = coll.GetComponent<BambooLeafPlate_Gimmick>();
            if (bpp != null)
            {
                if (TypeNo == 5 || TypeNo == 6 || TypeNo == 7 || TypeNo == 8)
                {
                    if(PlateNo != bpp._plateNo)
                    {
                        PlateNo = bpp._plateNo;
                        RequestSerialization();
                    }
                }
            }
        }
    }

    public void FuncRoll()
    {
        _vRCPickup.Drop();

        if (_rigi != null)
        {
            Vector3 randomDirection = new Vector3(Random.Range(-_rollHorizon, _rollHorizon), _rollVertical, Random.Range(-_rollHorizon, _rollHorizon));
            _rigi.AddForce(randomDirection * forceStrength, ForceMode.Impulse);
        }
        SendCustomEventDelayedSeconds(nameof(Reset), 10.0f, VRC.Udon.Common.Enums.EventTiming.Update);
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            OniNo = OniNo;
            if (PaddleNo == -1 && PlateNo == -1)
            {
                PaddleNo = PaddleNo;
                PlateNo = PlateNo;
            }
            else if (PlateNo != -1) { PlateNo = PlateNo; }
            else if (PaddleNo != -1) { PaddleNo = PaddleNo; }

            MeshRFlg = MeshRFlg;
            TypeNo = TypeNo;
            GripCount = GripCount;
            ShapekeyInt = ShapekeyInt;
            RollFlg = RollFlg;
            PosSync = PosSync;
            RotSync = RotSync;
            RequestSerialization();
        }
    }

    public void UnsetParentConstraint()
    {
        if (_parentConstraint != null)
        {
            _parentConstraint.constraintActive = false;
            for (int i = _parentConstraint.sourceCount - 1; i >= 0; --i)
                _parentConstraint.RemoveSource(i);
        }
    }

    public void Reset()
    {
        _vRCPickup.Drop();
        _psExplosion.SetActive(false);
        MeshRFlg = false;
        RollFlg = false;
        PosSync = Vector3.zero;
        RotSync = Quaternion.identity;
        TypeNo = 0;
        ShapekeyInt = 0;
        GripCount = 0;
        PlateNo = -1;
        PaddleNo = -1;
        RequestSerialization();
        _sub.gameObject.transform.localPosition = Vector3.zero;
        _sub.gameObject.transform.localRotation = Quaternion.identity;
    }

}
