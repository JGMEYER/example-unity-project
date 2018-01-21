using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockDropSpawner : MonoBehaviour {
	[SerializeField] private GameObject _rockPrefab;

	public float minSpawnDelay = 3f;
	public float maxSpawnDelay = 6f;
	private float[] _pattern;
	private int _currentPattern = -1;
	private float _timeSinceLastSpawn;
	private float _nextSpawn;
	private bool _done = false;

	void Start () {
	}
	
	void Update () {
		if (_pattern == null) return;

		_timeSinceLastSpawn += Time.deltaTime;

		if (_timeSinceLastSpawn > _nextSpawn) {
			GameObject rock = Instantiate(_rockPrefab) as GameObject;
			rock.transform.position = transform.position;

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
