using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

	public Slider healthSlider;
	public float totalHealth = 100f;
	public float currentHealth;

	void Awake() {
		currentHealth = totalHealth;
	}

	void Start() {
		healthSlider.value = currentHealth;		
	}

	public bool playerIsDead() {
		return currentHealth <= 0f;
	}

	public void TakeDamage(float damage) {
		currentHealth -= damage;

		if (currentHealth <= 0f) {
			currentHealth = 0f;
		}

		healthSlider.value = currentHealth;
	}

}
