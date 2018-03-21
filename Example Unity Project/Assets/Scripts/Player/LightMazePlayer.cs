using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMazePlayer : Player {

	[SerializeField]
	private ParticleSystem _deathExplosion;

	[Header("Movement")]
	public bool canMove = true;
	public bool canWallJump = false;
	public float horizontalSpeed = 8f;
	public float initialJumpVelocity = 3f;
	public float maxJumpHoldTime = 0.8f;
	public float fallGravityMultiplier = 2.5f;

	[Header("Detectors")]
	public float rayCastDist = 0.27f;

	private Rigidbody _rb;

	private float _inputHorizontal = 0;
	private int _inputVertical = 0;

	private float _jumpHoldCounter = 0;
	private bool _canJump = true;
	private bool _isDead = false;

	private LightMazeJetpack jetpack = null;

	void Start() {
		_rb = GetComponent<Rigidbody>();
	}

	void Update() {
		if (!_isDead) {
			DoInput();
		}
	}

	void FixedUpdate() {
		if (canMove) {
			if (HasJetpack()) {
				DoJetpackMovement();
			} else {
				DoMovement();
			}
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, Vector3.down * rayCastDist);
		Gizmos.DrawRay(transform.position, Vector3.left * rayCastDist);
		Gizmos.DrawRay(transform.position, Vector3.right * rayCastDist);

		if (_canJump) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(transform.position, 0.1f);
		}

		if (Application.isPlaying) {
			Gizmos.color = Color.cyan;
			Gizmos.DrawRay(transform.position, _rb.velocity);
		}
	}

	void DoInput() {
		if (_controls.GetJump()) {
			_jumpHoldCounter -= Time.deltaTime;
		}

		if (_controls.GetJump() && _canJump) {
			_inputVertical = 1;
			_jumpHoldCounter = maxJumpHoldTime;
			_canJump = false;
		}

		_inputHorizontal = _controls.GetHorizontal();
	}

	void DoMovement() {
		Vector3 resetHorizontalVelocity = new Vector3(0, _rb.velocity.y, 0);
		_rb.velocity = resetHorizontalVelocity;

		_rb.velocity += Vector3.up * _inputVertical * initialJumpVelocity * Time.deltaTime;

		if (_rb.velocity.y < 0 || !_controls.GetJump() || _jumpHoldCounter <= 0) {
			_rb.velocity += Vector3.up * Physics.gravity.y * (fallGravityMultiplier - 1) * Time.deltaTime;
		}

		_rb.velocity += Vector3.right * _inputHorizontal * horizontalSpeed * Time.deltaTime;

		_inputHorizontal = 0;
		_inputVertical = 0;
	}

	void DoJetpackMovement() {
		// I am sure I will regret this design choice
		Vector3 jetpackVelocity = jetpack.GetVelocity(_inputHorizontal);
		_rb.velocity = jetpackVelocity;
	}

	void OnCollisionEnter(Collision collision) {
		CheckCanJump(collision);
	}

	void OnCollisionStay(Collision collision) {
		CheckCanJump(collision);
	}

	void OnCollisionExit(Collision collision) {
		if (!_canJump) {
			return;
		}

		RaycastHit hit;

		bool collisionBelow = false;
		if (Physics.Raycast(transform.position, Vector3.down, out hit)) {
			collisionBelow |= hit.distance <= rayCastDist;
		}

		_canJump &= collisionBelow;
	}

	void CheckCanJump(Collision collision) {
		if (collision.gameObject.name.StartsWith("Player")) {
			Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
			return;
		}

		RaycastHit hit;

		bool collisionBelow = false;
		if (Physics.Raycast(transform.position, Vector3.down, out hit)) {
			collisionBelow |= hit.distance <= rayCastDist;
		}

		_canJump |= collisionBelow;

		if (canWallJump) {
			bool collisionLeft = false;
			if (Physics.Raycast(transform.position, Vector3.left, out hit)) {
				collisionLeft |= hit.distance <= rayCastDist;
			}

			bool collisionRight = false;
			if (Physics.Raycast(transform.position, Vector3.right, out hit)) {
				collisionRight |= hit.distance <= rayCastDist;
			}

			_canJump |= (collisionLeft || collisionRight);
		}
	}

	void OnTriggerEnter(Collider other) {
		LightMazeJetpack jetpackItem = other.GetComponent<LightMazeJetpack>();

		if (jetpackItem != null && !jetpackItem.IsEquipped()) {
			EquipJetpack(jetpackItem);
		}
	}

	void EquipJetpack(LightMazeJetpack jetpackItem) {
		jetpack = jetpackItem;
		jetpack.SetEquipped(true);

		_rb.useGravity = false;
		_rb.velocity = Vector3.zero;
		_rb.angularVelocity = Vector3.zero;
		_rb.transform.rotation = Quaternion.Euler(Vector3.zero);

		// I am sure I will regret this design choice
		transform.position = jetpack.transform.position;
		jetpack.transform.parent = transform;
		jetpack.transform.localPosition = Vector3.zero;

		Vector3 newPosition = transform.position;
		newPosition.z = -3f; // arbitrary
		transform.position = newPosition;
	}

	public bool HasJetpack() {
		return (jetpack != null);
	}

	public void Kill(bool explode) {
		_rb.velocity = Vector3.zero;
		_rb.useGravity = false;
		_isDead = true;
		canMove = false;

		if (explode) {
			_deathExplosion.Emit(5);
		}
	}

	public bool IsDead() {
		return _isDead;
	}

}
