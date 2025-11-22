using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ShapedRiceBall_Manager : UdonSharpBehaviour
{
    [HideInInspector] public ShapedRiceBall_Pickup[] _objs;
    public Transform _pool;
    [SerializeField] GameObject _lidObj;
    [SerializeField] GameObject _psObj;
    [SerializeField] GameObject _mrFull;
    [SerializeField] GameObject _mrEmpty;

    [Header("処理間隔（フレーム単位）")]
    [SerializeField] int _checkIntervalFrames = 5; // ← 何フレームごとにチェックするか
    int _frameCounter = 0; // 内部カウンタ

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(ShowFlg))] bool _showFlg = false;

    public bool ShowFlg
    {
        get => _showFlg;
        set
        {
            _showFlg = value;
            _mrFull.SetActive(value);
            _psObj.SetActive(value);
            _mrEmpty.SetActive(!value);
        }
    }

    void Update()
    {
        if (!Networking.LocalPlayer.IsOwner(gameObject)) return;

        // --- 指定フレーム間隔ごとに実行 ---
        _frameCounter++;
        if (_frameCounter < _checkIntervalFrames) return;
        _frameCounter = 0; // リセット

        // --- チェック処理本体 ---
        if (_lidObj.transform.localPosition != Vector3.zero)
        {
            for (int i = 0; i < _objs.Length; i++)
            {
                if (_objs[i].transform.localPosition == Vector3.zero)
                {
                    ShowFlg = true;
                    RequestSerialization();
                    return;
                }
            }
            ShowFlg = false;
            RequestSerialization();
        }
        else
        {
            ShowFlg = false;
            RequestSerialization();
        }
    }
}
