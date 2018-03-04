using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMazePlayer : MonoBehaviour {

	[SerializeField]
	private ParticleSystem _deathExplosion;

	public KeyCode upKey;
	public KeyCode leftKey;
	public KeyCode rightKey;

	public bool canWallJump = false;
	public float horizontalSpeed = 8f;
	public float initialJumpVelocity = 3f;
	public float jumpGravityMultiplier = 1f;
	public float fallGravityMultiplier = 2.5f;
	public float rayCastDist = 0.27f;

	private Rigidbody _rb;
	private int _inputHorizontal = 0;
	private int _inputVertical = 0;
	private bool _canJump = true;
	private bool _isDead = false;
	private bool _isVisible = true;

	void Start() {
		_rb = GetComponent<Rigidbody>();
	}

	void Update() {
		if (!_isDead) {
			DoInput();
		}
	}

	void FixedUpdate() {
		DoMovement();		
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
		if (Input.GetKey(upKey) && _canJump) {
			_inputVertical = 1;
			_canJump = false;
		}

		if (Input.GetKey(leftKey)) {
			_inputHorizontal = -1;
		}
		if (Input.GetKey(rightKey)) {
			_inputHorizontal = 1;
		}
	}

	void DoMovement() {
		if (_inputVertical != 0) {
			_rb.velocity = new Vector3(_rb.velocity.x, _inputVertical * initialJumpVelocity, _rb.velocity.z);
		}
		if (_rb.velocity.y < 0) {
			_rb.velocity += Vector3.up * Physics.gravity.y * (fallGravityMultiplier - 1) * Time.deltaTime;
		} else if (_rb.velocity.y > 0 && !Input.GetKey(upKey)) {
			_rb.velocity += Vector3.up * Physics.gravity.y * (jumpGravityMultiplier - 1) * Time.deltaTime;
		}

		_rb.velocity += Vector3.right * _inputHorizontal * horizontalSpeed * Time.deltaTime;

		_inputHorizontal = 0;
		_inputVertical = 0;

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
			if (hit.distance <= rayCastDist) {
				collisionBelow = true;
			}
		}

		if (!collisionBelow) {
			_canJump = false;
		}
	}

	void CheckCanJump(Collision collision) {
		if (collision.gameObject.name.StartsWith("Player")) {
			Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
			return;
		}

		RaycastHit hit;

		bool collisionBelow = false;
		if (Physics.Raycast(transform.position, Vector3.down, out hit)) {
			if (hit.distance <= rayCastDist) {
				collisionBelow = true;
			}
		}

		if (collisionBelow) {
			_canJump = true;
		}

		if (canWallJump) {
			bool collisionLeft = false;
			if (Physics.Raycast(transform.position, Vector3.left, out hit)) {
				if (hit.distance <= rayCastDist) {
					collisionLeft = true;
				}
			}

			bool collisionRight = false;
			if (Physics.Raycast(transform.position, Vector3.right, out hit)) {
				if (hit.distance <= rayCastDist) {
					collisionRight = true;
				}
			}

			if (collisionLeft || collisionRight) {
				_canJump = true;
			}
		}
	}

	public bool IsVisible() {
		return _isVisible;
	}

	public void OnBecameVisible() {
		_isVisible = true;
	}

	public void OnBecameInvisible() {
		_isVisible = false;
	}

	public void Kill(bool explode) {
		_rb.velocity = new Vector3(0, 0, 0);
		_rb.useGravity = false;
		_isDead = true;

		if (explode) {
			_deathExplosion.Emit(5);
		}
	}

	public bool IsDead() {
		return _isDead;
	}

}
