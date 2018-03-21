using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour {

	public GameObject targetObject;
	public Vector3 offsetFromObject;
	
	void Update () {
		Vector3 targetPosition = targetObject.transform.position;
		transform.position = targetPosition + offsetFromObject;
	}

}
