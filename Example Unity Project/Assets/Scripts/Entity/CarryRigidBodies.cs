using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryRigidBodies : MonoBehaviour {

	public List<Rigidbody> rigidbodies = new List<Rigidbody>();

	private Vector3 _lastPosition;
	private Transform _transform;

	void Start () {
		_transform = transform;
		_lastPosition = _transform.position;	
	}
	
	void LateUpdate () {
		foreach (Rigidbody rb in rigidbodies) {
			Vector3 velocity = (_transform.position - _lastPosition);
			rb.transform.Translate(velocity, _transform);
		}

		_lastPosition = _transform.position;
	}

	void OnTriggerEnter(Collider other) {
		Rigidbody rb = other.GetComponent<Collider>().GetComponent<Rigidbody>();	
		if (rb != null) {
			Add(rb);
		}
	}

	void OnTriggerExit(Collider other) {
		Rigidbody rb = other.GetComponent<Collider>().GetComponent<Rigidbody>();	
		if (rb != null) {
			Remove(rb);
		}
	}

	void Add(Rigidbody rb) {
		if (!rigidbodies.Contains(rb)) {
			rigidbodies.Add(rb);
		}
	}

	void Remove(Rigidbody rb) {
		if (rigidbodies.Contains(rb)) {
			rigidbodies.Remove(rb);
		}
	}
}
