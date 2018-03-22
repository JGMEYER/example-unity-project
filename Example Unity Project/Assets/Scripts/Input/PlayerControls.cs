using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : Object {

	// Only UI should pull these values. Players should call included methods
	// in order to keep all player input logic controller agnostic.
	public KeyCode UpKey { get; private set; }
	public KeyCode LeftKey { get; private set; }
	public KeyCode DownKey { get; private set; }
	public KeyCode RightKey { get; private set; }

	public PlayerControls(KeyCode upKey, KeyCode leftKey, KeyCode downKey,
						  KeyCode rightKey) {
		UpKey = upKey;
		LeftKey = leftKey;
		DownKey = downKey;
		RightKey = rightKey;
	}

	public float GetHorizontal() {
		float horizontal = 0f;

		if (Input.GetKey(LeftKey)) {
			horizontal += -1;
		}
		if (Input.GetKey(RightKey)) {
			horizontal += 1;
		}

		return horizontal;
	}

	public float GetVertical() {
		float vertical = 0f;

		if (Input.GetKey(UpKey)) {
			vertical += 1;
		}
		if (Input.GetKey(DownKey)) {
			vertical += -1;
		}

		return vertical;
	}

	public bool GetJump() {
		return Input.GetKey(UpKey);
	}

	public bool GetUpKey() {
		return Input.GetKey(UpKey);
	}

	public bool GetUpKeyDown() {
		return Input.GetKeyDown(UpKey);
	}

	public bool GetDownKey() {
		return Input.GetKey(DownKey);	
	}

	public bool GetDownKeyDown() {
		return Input.GetKeyDown(DownKey);
	}

}
