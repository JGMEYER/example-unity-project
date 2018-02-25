using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMazePlayer : MonoBehaviour {

	public KeyCode upKey;
	public KeyCode leftKey;
	public KeyCode rightKey;
	public float acceleration;
	public float verticalAccelerationMultiplier;
	public float maxSpeed = 8;

	private Renderer rend;
	private Rigidbody rb;
	private bool canJump = true;
	private float _rayCastDist = 0.27f;

	void Start() {
		rend = GetComponent<Renderer>();
		rb = GetComponent<Rigidbody>();
	}

	void Update() {
		HandleInput();
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, Vector3.down * _rayCastDist);
		Gizmos.DrawRay(transform.position, Vector3.left * _rayCastDist);
		Gizmos.DrawRay(transform.position, Vector3.right * _rayCastDist);

		if (canJump) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(transform.position, 0.1f);
		}
	}

	private void HandleInput() {
		float moveHorizontal = 0;
		float moveVertical = 0;

		if (canJump && Input.GetKey(upKey)) {
			moveVertical += verticalAccelerationMultiplier;
			canJump = false;
		}
		if (Input.GetKey(leftKey)) {
			moveHorizontal -= 1;
		}
		if (Input.GetKey(rightKey)) {
			moveHorizontal += 1;
		}

		Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0);
		rb.AddForce(movement * acceleration);

		if (rb.velocity.magnitude > maxSpeed) {
			rb.velocity = rb.velocity.normalized * maxSpeed;
		}
	}

	private void OnCollisionEnter(Collision collision) {
		CheckCanJump(collision);
	}

	private void OnCollisionStay(Collision collision) {
		CheckCanJump(collision);
	}

	private void CheckCanJump(Collision collision) {
		if (collision.gameObject.name.StartsWith("Player")) {
			Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
			return;
		}

		RaycastHit hit;

		bool collisionLeft = false;
		if (Physics.Raycast(transform.position, Vector3.left, out hit)) {
			if (hit.distance <= _rayCastDist) {
				collisionLeft = true;
			}
		}

		bool collisionRight = false;
		if (Physics.Raycast(transform.position, Vector3.right, out hit)) {
			if (hit.distance <= _rayCastDist) {
				collisionRight = true;
			}
		}

		bool collisionBelow = false;
		if (Physics.Raycast(transform.position, Vector3.down, out hit)) {
			if (hit.distance <= _rayCastDist) {
				collisionBelow = true;
			}
		}

		if (collisionBelow || collisionLeft || collisionRight) {
			canJump = true;
		}
	}

	private void OnCollisionExit(Collision collision) {
		if (!canJump) {
			return;
		}

		RaycastHit hit;

		bool collisionBelow = false;
		if (Physics.Raycast(transform.position, Vector3.down, out hit)) {
			if (hit.distance <= _rayCastDist) {
				collisionBelow = true;
			}
		}

		if (!collisionBelow) {
			canJump = false;
		}
	}

}