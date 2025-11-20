
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class Glitter_ShootingStar : UdonSharpBehaviour
{
	[SerializeField] float _timer;
	[SerializeField] float _shootR;
    [SerializeField] float _h;
	[SerializeField] GameObject _shootPos;
	[SerializeField] GameObject _shootTarget;
    public VRCObjectPool _ssObjOP;
	public GameObject[] _ssObjArr;
	private float _rHalf;
    private float _count;
    private float _timerRandom;

	void Start()
    {
		_rHalf = _shootR / 2f;
        _timerRandom = Random.Range(_timer, _timer + 5f);
    }

    void Update()
    {
        _count += Time.deltaTime;
        if (_timerRandom <= _count)
        {
            RandomShooting();
			_count = 0;
            _timerRandom = Random.Range(_timer, _timer + 5f);
        }
    }

	private void RandomShooting()
    {
        GameObject obj = _ssObjOP.TryToSpawn();
        if (obj == null) return;
        obj.transform.position = _shootPos.transform.localPosition;
        obj.transform.forward = (_shootTarget.transform.localPosition - _shootPos.transform.localPosition).normalized;
        float anglePos = Random.Range(0, Mathf.PI * 2f);
        _shootPos.transform.localPosition = new Vector3((float)(Mathf.Sin(anglePos)) * _shootR, _h, (float)(Mathf.Cos(anglePos) * _shootR));
        _shootTarget.transform.localPosition = new Vector3(Random.Range(-_rHalf, _rHalf), _h, Random.Range(-_rHalf, _rHalf));
    }

    public void AllReSpawnObj()
    {
        for (int i = 0; i < _ssObjArr.Length; i++)
        {
            GameObject obj = _ssObjArr[i];
            _ssObjOP.Return(obj);
        }
    }
}
