
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class RiceSet_Manager : UdonSharpBehaviour
{
    [Header("=====笹皿=====")]
    public BambooLeafPlate_Manager _bpm;
    public GameObject _bpmPrefab;

    [Header("=====しゃもじ=====")]
    public RicePaddle_Manager _rpm;
    public GameObject _rpmPrefab;

    [Header("=====おにぎり=====")]
    public ShapedRiceBall_Manager _rbm;
    public GameObject _rbmPrefab;

    [Header("=====爆発確率=====")]
    [Range(0, 100), SerializeField] public int _explosionProbability = 0;
    [Header("=====転がる確率=====")]
    [Range(0, 100), SerializeField] public int _rollProbability = 0;

}
