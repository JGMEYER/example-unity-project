using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockThrowSpawner : MonoBehaviour {

	[SerializeField]
	private ThrownItem _thrownRockPrefab;

	public float rockThrowDistanceZ = 15f;
	public float rockThrowAmplitude = 15f;
	public float rockThrowSpeed = 4f;

	private float[] _pattern;
	private int _currentPattern = -1;
	private float _timeSinceLastSpawn;
	private float _nextSpawn;
	private bool _done = false;

	void Update() {
		if (_pattern == null) return;

		if (!_done) {
			_timeSinceLastSpawn += Time.deltaTime;

			if (_timeSinceLastSpawn > _nextSpawn) {
				SpawnRock();
				NextTimer();
			}
		}
	}

	void SpawnRock() {
		ThrownItem thrownRock = Instantiate(_thrownRockPrefab) as ThrownItem;
		thrownRock.GetComponent<ThrownItem>().Initialize(transform.position.x, rockThrowDistanceZ, rockThrowAmplitude, rockThrowSpeed);
		thrownRock.transform.parent = transform;

		FindObjectOfType<AudioManager>().Play("Throw");
	}

	void NextTimer() {
		if (_currentPattern + 1 >= _pattern.Length) {
			_nextSpawn = float.MaxValue;
			_done = true;
			return;
		}

		_timeSinceLastSpawn = 0;
		_currentPattern += 1;
		_nextSpawn = _pattern[_currentPattern];
	}

	public void Initialize(float[] pattern) {
		_pattern = pattern;
		NextTimer();
	}

	public void Stop() {
		_done = true;

		foreach (ThrownItem activeRock in GetComponentsInChildren<ThrownItem>()) {
			Destroy(activeRock.gameObject);
		}
	}

}
