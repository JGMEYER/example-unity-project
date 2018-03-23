using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	[SerializeField]
	protected PlayerNumber _playerNumber;

	protected IPlayerControls _controls { get; private set; }

	public void Awake() {
		_controls = GameControlsManager.Instance.PlayerControls(_playerNumber);
	}

	public void OnValidate() {
		if (_playerNumber == 0) {
			throw new System.ArgumentException("Player is missing PlayerNumber assignment.");
		}
	}

}
