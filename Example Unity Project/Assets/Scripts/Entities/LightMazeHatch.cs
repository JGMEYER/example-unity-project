using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMazeHatch : MonoBehaviour {

	public float smooth = 0.5f;

	[SerializeField]
	private GameObject hatch;
	[SerializeField]
	private GameObject lazer;

	bool _activated = false;

	private void OnTriggerEnter(Collider other) {
		if (!_activated) {
			lazer.gameObject.SetActive(false);

			Vector3 hatchScale = hatch.transform.localScale;
			Vector3 activatedHatchScale = new Vector3(1, hatchScale.y, hatchScale.z);

			hatch.transform.localScale = activatedHatchScale;
			hatch.gameObject.SetActive(true);
			_activated = true;
		}
	}

}
