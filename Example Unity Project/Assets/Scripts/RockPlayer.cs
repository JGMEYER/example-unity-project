using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockPlayer : MonoBehaviour {

	public KeyCode playerKey;
	public Material defaultMaterial;
	public Material shieldMaterial;
	public int shieldDamage = 5;
	public int hurtDamage = 15;
	public float shieldDuration = 0.2f;
	public float hurtDuration = 0.2f;
	public float hurtShakeSpeed = 50f;
	public float hurtShakeAmplitude = 0.1f;
	public float hurtSquashRatio = 0.8f;
	private bool _shieldActive = false;
	private bool _isHurt = false;
	private float _hurtTimer = 0f;
	private Vector3 _originalPosition;
	private Vector3 _originalScale;

	private void Awake() {
		_originalPosition = transform.position;
		_originalScale = transform.localScale;
	}

	void Start () {
		GetComponent<Renderer>().material = defaultMaterial;
	}
	
	void Update () {
		if (Input.GetKeyDown(playerKey) && !_shieldActive && !_isHurt) {
			StartCoroutine(ActivateShield());
		}	

		if (_isHurt) {
			float shakeX = -1 * Mathf.Cos(_hurtTimer * hurtShakeSpeed) * hurtShakeAmplitude;
			transform.position = new Vector3(_originalPosition.x + shakeX, transform.position.y, _originalPosition.z);
			_hurtTimer += Time.deltaTime;

            IdleAnim idleAnim = GetComponent<IdleAnim>();
			if (idleAnim) {
				idleAnim.Restart();
			}
		}
	}

	private void OnTriggerEnter(Collider other) {
		ThrownItem thrownItem = other.GetComponent<ThrownItem>();
		if (thrownItem != null) {
			if (!_shieldActive) {
				StartCoroutine(Hurt());
			}
			Destroy(thrownItem.gameObject);
		}
	}

	private IEnumerator ActivateShield() {
		GetComponent<PlayerHealth>().TakeDamage(shieldDamage);

		_shieldActive = true;
		GetComponent<Renderer>().material = shieldMaterial;

		yield return new WaitForSeconds(shieldDuration);

		_shieldActive = false;
		GetComponent<Renderer>().material = defaultMaterial;
	}

	private IEnumerator Hurt() {
		GetComponent<PlayerHealth>().TakeDamage(hurtDamage);
        FindObjectOfType<AudioManager>().Play("Hit");

        _isHurt = true;
		_hurtTimer = 0f;
		transform.localScale = new Vector3(_originalScale.x, _originalScale.y * hurtSquashRatio, _originalScale.z);
		transform.position = new Vector3(_originalPosition.x, _originalPosition.y - _originalScale.y * (1 - hurtSquashRatio) / 2, _originalPosition.z);
		GetComponent<Renderer>().material.color = Color.red;

		yield return new WaitForSeconds(hurtDuration);

		_isHurt = false;
		transform.localScale = _originalScale;
		transform.position = _originalPosition;
		if (_shieldActive) {
			GetComponent<Renderer>().material.color = shieldMaterial.color;
		} else {
			GetComponent<Renderer>().material.color = defaultMaterial.color;
		}
	}

}
