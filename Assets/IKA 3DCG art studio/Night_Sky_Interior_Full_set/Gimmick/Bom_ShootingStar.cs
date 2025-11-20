
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

public class Bom_ShootingStar : UdonSharpBehaviour
{
    public GameObject _shootPos;
    public GameObject _shootTarget;
    public VRCObjectPool _starObjOP;
    public GameObject[] _starObjArr;
    [SerializeField] float _timer;
    [SerializeField] float _shootR;
    [SerializeField] float _h;
    [SerializeField] float _lifeTimerAny;
    private float _rHalf = 0;
    private float _count = 0;
    private float _timerRandom = 0;

    void Start()
    {
        _rHalf = _shootR / 2f;
        _timerRandom = Random.Range(_timer, _timer + 5f);
        RandomShootingStart();
        CheckStarFlgStart();
    }

    public void RandomShootingStart()
    {

        if (Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            SendCustomEventDelayedSeconds(nameof(RandomShooting), _timerRandom, VRC.Udon.Common.Enums.EventTiming.Update);
        }
    }

    public void CheckStarFlgStart()
    {
        if (Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            SendCustomEventDelayedSeconds(nameof(CheckStarFlg), 2f, VRC.Udon.Common.Enums.EventTiming.Update);
        }
    }

    public void RandomShooting()
    {
        GameObject obj = _starObjOP.TryToSpawn();
        if (obj == null)
        {            
            return;
        }
        obj.transform.position = _shootPos.transform.position;
        obj.transform.forward = (_shootTarget.transform.position - _shootPos.transform.position).normalized;
        foreach (Transform item in obj.transform)
        {
            item.localPosition = Vector3.zero;
            item.localRotation = Quaternion.identity;
        }
        //初期化
        ShootingStarMeteor_Pickup ssmp = obj.transform.GetChild(0).GetComponent<ShootingStarMeteor_Pickup>();
        ssmp.LifeTimer = Random.Range(_lifeTimerAny, _lifeTimerAny + 3f);
        ssmp._pickupRigi.velocity = ssmp._pickupObj.transform.forward * 10;
        ssmp.PickupFlg = false;
        ssmp._pickupRigi.isKinematic = false;

        float anglePos = Random.Range(0, Mathf.PI * 2f);
        _shootPos.transform.localPosition = new Vector3((float)(Mathf.Sin(anglePos)) * _shootR, _h, (float)(Mathf.Cos(anglePos) * _shootR));
        _shootTarget.transform.localPosition = new Vector3(Random.Range(-_rHalf, _rHalf), _h, Random.Range(-_rHalf, _rHalf));

        _timerRandom = Random.Range(_timer, _timer + 5f);
        SendCustomEventDelayedSeconds(nameof(RandomShooting), _timerRandom, VRC.Udon.Common.Enums.EventTiming.Update);
    }

    public void CheckStarFlg()
    {
        for (int i = 0; i < _starObjArr.Length; i++)
        {
            ShootingStarMeteor_Pickup ssmp = _starObjArr[i].transform.GetChild(0).GetComponent<ShootingStarMeteor_Pickup>();
            if (ssmp == null) continue;
            if ((!ssmp.PickupFlg) && (ssmp.LifeTimer <= ssmp.LifeCount))
            {
                VRCPickup pickup = (VRCPickup)_starObjArr[i].transform.GetChild(0).GetComponent(typeof(VRCPickup));
                if (pickup != null)
                {
                    pickup.Drop();
                }
                Networking.SetOwner(Networking.LocalPlayer, ssmp.gameObject);
                ssmp._pickupRigi.velocity = Vector3.zero;
                ssmp._pickupRigi.angularVelocity = Vector3.zero;
                ssmp.PickupFlg = false;
                ssmp._pickupObj.transform.localPosition = Vector3.zero;
                ssmp._pickupObj.transform.localRotation = Quaternion.identity;
                ssmp.LifeCount = 0;
                ssmp._pickupRigi.isKinematic = false;
                _starObjOP.Return(_starObjArr[i]);
            }
        }
        SendCustomEventDelayedSeconds(nameof(CheckStarFlg), 2f, VRC.Udon.Common.Enums.EventTiming.Update);
    }
}
