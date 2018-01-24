using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownItem : MonoBehaviour {

	private float _targetX;
	private float _distanceZ;
	private float _amplitude;
	private float _speed;
	private float _a;  // width modifier for parabola
	private float _currentZ;

	void Start () {
	}
	
	void Update () {
		_currentZ -= Time.deltaTime * _speed;

		float y = -1 * _a * Mathf.Pow(_currentZ - (_distanceZ / 2), 2) + _amplitude;
		transform.position = new Vector3(_targetX, y, _currentZ);
	}

	public void Initialize(float targetX, float distanceZ, float amplitude, float speed) {
		_targetX = targetX;
		_distanceZ = distanceZ;
		_amplitude = amplitude;
		_speed = speed;

		// solve for A using (0, 0, 0)
		_a = -1 * (-1 * _amplitude / Mathf.Pow(-distanceZ / 2, 2));

		_currentZ = _distanceZ;
	}

}
