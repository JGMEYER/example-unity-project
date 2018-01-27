using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

	public Slider healthSlider;
	public int totalHealth = 100;
	public int currentHealth;

	void Awake() {
		currentHealth = totalHealth;
	}

	void Start () {
		healthSlider.value = currentHealth;		
	}

	public bool isDead () {
		return currentHealth <= 0;
	}

	public void TakeDamage (int damage) {
		currentHealth -= damage;

		if (currentHealth <= 0) {
			currentHealth = 0;
		}

		healthSlider.value = currentHealth;
	}

}
