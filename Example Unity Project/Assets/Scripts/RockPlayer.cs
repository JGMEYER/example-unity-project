using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockPlayer : MonoBehaviour {

	public KeyCode playerKey;
	public Material defaultMaterial;
	public Material shieldMaterial;
	public float shieldDuration = 0.2f;
	public float hurtDuration = 0.2f;
	public float hurtShakeSpeed = 50f;
	public float hurtShakeAmplitude = 0.1f;
	public float hurtPlayerSize = 1.5f;
	private bool _shieldActive = false;
	private bool _isHurt = false;
	private float _hurtTimer = 0f;
	private Vector3 _originalPosition;

	void Start () {
		_originalPosition = transform.position;
		GetComponent<Renderer>().material = defaultMaterial;
	}
	
	void Update () {
		if (Input.GetKeyDown(playerKey) && !_shieldActive && !_isHurt) {
			StartCoroutine(ActivateShield());
		}	

		if (_isHurt) {
			float shakeX = -1 * Mathf.Cos(_hurtTimer * hurtShakeSpeed) * hurtShakeAmplitude;
			transform.position = new Vector3(_originalPosition.x + shakeX, _originalPosition.y, _originalPosition.z);
			_hurtTimer += Time.deltaTime;
		}
	}

	private void OnTriggerEnter(Collider other) {
		Rock rock = other.GetComponent<Rock>();
		ThrownRock thrownRock = other.GetComponent<ThrownRock>();
		if (rock != null || thrownRock != null) {
			if (!_shieldActive) {
				StartCoroutine(Hurt());
			}
			if (rock != null) Destroy(rock.gameObject);
			if (thrownRock != null) Destroy(thrownRock.gameObject);
		}
	}

	private IEnumerator ActivateShield() {
		_shieldActive = true;
		GetComponent<Renderer>().material = shieldMaterial;

		yield return new WaitForSeconds(shieldDuration);

		_shieldActive = false;
		GetComponent<Renderer>().material = defaultMaterial;
	}

	private IEnumerator Hurt() {
		_isHurt = true;
		_hurtTimer = 0f;
		transform.localScale = Vector3.one * hurtPlayerSize;
		GetComponent<Renderer>().material.color = Color.red;

		yield return new WaitForSeconds(hurtDuration);

		_isHurt = false;
		transform.localScale = Vector3.one;
		transform.position = _originalPosition;
		if (_shieldActive) {
			GetComponent<Renderer>().material.color = shieldMaterial.color;
		} else {
			GetComponent<Renderer>().material.color = defaultMaterial.color;
		}
	}

}
