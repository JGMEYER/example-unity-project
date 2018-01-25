using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurneyWobbleyIdle : IdleAnim {

	public float turnSpeed = 2f;
	public float maxTurnAngle = 10f;
	public float wobbleSpeed = 1f;
	public float maxWobbleAngle = 2f;
	private float _turnTimer = 0f;

	void Start () {
	}
	
	void Update () {
		_turnTimer += Time.deltaTime;

		float turnY = Mathf.Sin(_turnTimer * turnSpeed) * maxTurnAngle;
		transform.eulerAngles = new Vector3(0, turnY, 0);
	}

	public new void Restart() {
		_turnTimer = 0f;
	}
}
