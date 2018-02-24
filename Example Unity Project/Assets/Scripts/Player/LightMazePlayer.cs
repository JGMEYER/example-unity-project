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

	void Start() {
		rend = GetComponent<Renderer>();
		rb = GetComponent<Rigidbody>();
	}

	void Update() {
		HandleInput();
	}

	private void OnDrawGizmosSelected() {
		// Vector3 center = rend.bounds.center;
		// float radius = rend.bounds.extents.magnitude;
		// Gizmos.color = Color.white;
		// Gizmos.DrawWireSphere(center, radius);
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

		Debug.Log(movement);
		Debug.Log(canJump);

		rb.AddForce(movement * acceleration);

		if (rb.velocity.magnitude > maxSpeed) {
			rb.velocity = rb.velocity.normalized * maxSpeed;
		}
	}

	private void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.name.StartsWith("Player")) {
			Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
			return;
		}

		Debug.Log("colliding!");

		Vector3 collisionNormal = collision.contacts[0].normal;

		canJump = true;

		//if (collisionNormal.y > 0) {
		//	canJump = true;
		//}

		//Debug.Log(collisionNormal);
		//Debug.Log(canJump);
	}

	/*
	public bool IsGrounded() {
		float distToGround = rb.collider.bounds.extents.y;
		return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1);
	}
	*/

}