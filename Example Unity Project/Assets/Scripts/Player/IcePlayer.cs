using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class IcePlayer : Player {

	[SerializeField]
	private Text _lifeText;

	public float acceleration;
	public Vector3 spawnPoint;
	public float collisionForce;
	public int numLife = 3;
	public float maxSpeed = 8;

	private Rigidbody _rb;

	private Vector3 _currentPosition;
	private float _inputHorizontal;
	private float _inputVertical;

	private int _floorHeight = 2;
	private int _resetHeight = -20;

	void Start() {
		_rb = GetComponent<Rigidbody>();
		transform.position = spawnPoint;

		UpdateLifeText();
	}

	void Update() {
		_currentPosition = transform.position;

		if (_currentPosition.y < _resetHeight) {
			HandleDeath();
		}

		if (_currentPosition.y > _floorHeight) {
			DoInput();
		}
	}

	void FixedUpdate() {
		DoMovement();
	}

	void DoInput() {
		_inputHorizontal = _controls.GetHorizontal();
		_inputVertical = _controls.GetVertical();
	}

	void DoMovement() {
		Vector3 movement = new Vector3(_inputHorizontal, 0f, _inputVertical);

		_rb.AddForce(movement * acceleration);

		if (_rb.velocity.magnitude > maxSpeed) {
			_rb.velocity = _rb.velocity.normalized * maxSpeed;
		}
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.name.StartsWith("Player")) {
			Vector3 dir = collision.contacts[0].point - transform.position;
			dir = -dir.normalized;
			dir.y = 0;
			GetComponent<Rigidbody>().AddForce(dir * collisionForce);
			FindObjectOfType<AudioManager>().Play("Bump");
		}
	}

	void HandleDeath() {
		numLife--;
		UpdateLifeText();

		if (numLife > 0) {
			transform.position = spawnPoint;
			_rb.velocity = Vector3.zero;
			_rb.angularVelocity = Vector3.zero;
		}
		else {
			GameObject manager = GameObject.Find("IceGameManager");
			IceGameManager iceManager = (IceGameManager)manager.GetComponent(typeof(IceGameManager));
			iceManager.HandlePlayerDeath(this.name);
			Destroy(this.gameObject);
		}
	}

	void UpdateLifeText() {
		_lifeText.text = this.name + " Lives: " + numLife.ToString();
	}

}
