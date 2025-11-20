using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ShootingStar : UdonSharpBehaviour
{
    public GameObject _shootingStar01;
    public GameObject _shootingStar02;
    public GameObject _shootingStar03;
    public Transform _shootingStarBarrel;
    private float _timer;
    private float _randomTime;
    private GameObject _shootingTmp;


    void Start()
    {
        _randomTime = Random.Range(10.0f, 11.0f);
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_randomTime <= _timer)
        {
            _timer = 0;
            ShootingStarSelect();
            _shootingTmp.SetActive(true);

            _shootingTmp.transform.position = _shootingStarBarrel.transform.position;
            _shootingTmp.transform.localRotation = Quaternion.Euler(_shootingStarBarrel.eulerAngles);
        }
    }

    private void ShootingStarSelect()
    {
        if (!_shootingStar01.activeSelf)
        {
            _shootingTmp = _shootingStar01;
            return;
        }
        else if (!_shootingStar02.activeSelf)
        {
            _shootingTmp = _shootingStar02;
            return;
        }
        else if (!_shootingStar03.activeSelf)
        {
            _shootingTmp = _shootingStar03;
            return;
        }
    }
}