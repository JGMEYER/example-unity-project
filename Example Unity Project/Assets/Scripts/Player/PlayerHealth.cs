using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    [SerializeField]
    private Slider healthSlider;
    [SerializeField]
    private float totalHealth = 100f;
    [SerializeField]
    private float currentHealth;

    private void Awake()
    {
        currentHealth = totalHealth;
    }

    private void Start()
    {
        healthSlider.value = currentHealth;
    }

    public bool playerIsDead()
    {
        return currentHealth <= 0f;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
        }

        healthSlider.value = currentHealth;
    }

}
