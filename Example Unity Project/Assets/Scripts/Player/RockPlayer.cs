using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockPlayer : Player
{

    [Header("Prefabs")]
    public Material DefaultMaterial;
    public Material ShieldMaterial;
    [Header("Damage")]
    public int ShieldDamage = 5;
    public int HurtDamage = 15;
    [Header("Hurt Animation")]
    public float HurtDuration = 0.2f;
    public float HurtShakeSpeed = 50f;
    public float HurtShakeAmplitude = 0.1f;
    public float HurtSquashRatio = 0.8f;

    private Renderer rend;
    private PlayerHealth health;
    private bool shieldActive = false;
    private bool isHurt = false;
    private float hurtTimer = 0f;
    private Vector3 originalPosition;
    private Vector3 originalScale;

    private new void Awake()
    {
        base.Awake();

        rend = GetComponent<Renderer>();
        health = GetComponent<PlayerHealth>();
        originalPosition = transform.position;
        originalScale = transform.localScale;
    }

    private void Start()
    {
        rend.material = DefaultMaterial;
    }

    private void Update()
    {
        if (!active)
        {
            return;
        }

        if (IsDead())
        {
            active = false;
            return;
        }

        if (isHurt)
        {
            float shakeX = -1 * Mathf.Cos(hurtTimer * HurtShakeSpeed) * HurtShakeAmplitude;
            transform.position = new Vector3(originalPosition.x + shakeX, transform.position.y, originalPosition.z);
            hurtTimer += Time.deltaTime;

            IdleAnim idleAnim = GetComponent<IdleAnim>();
            if (idleAnim)
            {
                idleAnim.Restart();
            }
        }
        else
        {
            if (controls.GetDownKey())
            {
                SetShieldActive(true);
                health.TakeDamage(ShieldDamage * Time.deltaTime);
            }
            else
            {
                SetShieldActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ThrownItem thrownItem = other.GetComponent<ThrownItem>();
        if (thrownItem != null)
        {
            if (active && !shieldActive)
            {
                StartCoroutine(Hurt());
            }
            Destroy(thrownItem.gameObject);
        }
    }

    private void SetShieldActive(bool shieldActive)
    {
        if (this.shieldActive == shieldActive)
        {
            return;
        }

        this.shieldActive = shieldActive;

        if (this.shieldActive)
        {
            rend.material = ShieldMaterial;
        }
        else
        {
            rend.material = DefaultMaterial;
        }
    }

    private IEnumerator Hurt()
    {
        health.TakeDamage(HurtDamage);
        FindObjectOfType<AudioManager>().Play("Hit");

        isHurt = true;
        hurtTimer = 0f;
        transform.localScale = new Vector3(originalScale.x, originalScale.y * HurtSquashRatio, originalScale.z);
        transform.position = new Vector3(originalPosition.x, originalPosition.y - originalScale.y * (1 - HurtSquashRatio) / 2, originalPosition.z);
        GetComponent<Renderer>().material.color = Color.red;

        yield return new WaitForSeconds(HurtDuration);

        isHurt = false;
        transform.localScale = originalScale;
        transform.position = originalPosition;
        if (shieldActive)
        {
            GetComponent<Renderer>().material.color = ShieldMaterial.color;
        }
        else
        {
            GetComponent<Renderer>().material.color = DefaultMaterial.color;
        }
    }

    public bool IsDead()
    {
        return health.playerIsDead();
    }

}
