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

    // 任意フレームごとに処理
    [SerializeField] int _updateInterval = 5;
    // たまに1フレーム増やす確率（0〜1）
    [SerializeField] float _jitterChance = 0.1f;
    int _frameCounter;
    int _jitterOffset;

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
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            _frameCounter++;
            // 指定間隔 + ジッター に一致したときだけ処理
            if (_frameCounter % (_updateInterval + _jitterOffset) != 0)
            {
                return;
            }
            // 1フレーム増やす処理
            _jitterOffset = Random.value < _jitterChance ? 1 : 0;

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
}
