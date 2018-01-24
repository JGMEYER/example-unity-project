using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockThrowSpawner : MonoBehaviour {
	[SerializeField]
	private GameObject _thrownRockPrefab;

	private float[] _pattern;
	private int _currentPattern = -1;
	private float _timeSinceLastSpawn;
	private float _nextSpawn;
	private bool _done = false;

	void Start() {
	}

	void Update() {
		if (_pattern == null) return;

		_timeSinceLastSpawn += Time.deltaTime;

		if (_timeSinceLastSpawn > _nextSpawn) {
			GameObject thrownRock = Instantiate(_thrownRockPrefab) as GameObject;
			thrownRock.GetComponent<ThrownRock>().Initialize(transform.position.x, 15, 15, 4);

			NextTimer();
		}
	}

	public void Initialize(float[] pattern) {
		_pattern = pattern;
		_done = false;
		NextTimer();
	}

	private void NextTimer() {
		if (_currentPattern + 1 >= _pattern.Length) {
			_nextSpawn = float.MaxValue;
			_done = true;
			return;
		}

		_timeSinceLastSpawn = 0;
		_currentPattern += 1;
		_nextSpawn = _pattern[_currentPattern];
	}

}
