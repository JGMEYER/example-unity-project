using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class IcePlayer : Player
{

    [SerializeField]
    private Text lifeText;

    public float Acceleration;
    public Vector3 SpawnPoint;
    public float CollisionForce;
    public int NumLife = 3;
    public float MaxSpeed = 8;

    private Rigidbody rb;

    private Vector3 currentPosition;
    private float inputHorizontal;
    private float inputVertical;

    private int floorHeight = 2;
    private int resetHeight = -20;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        transform.position = SpawnPoint;

        UpdateLifeText();
    }

    private void Update()
    {
        currentPosition = transform.position;

        if (currentPosition.y < resetHeight)
        {
            HandleDeath();
        }

        if (currentPosition.y > floorHeight)
        {
            DoInput();
        }
    }

    private void FixedUpdate()
    {
        DoMovement();
    }

    private void DoInput()
    {
        inputHorizontal = controls.GetMovementHorizontal();
        inputVertical = controls.GetMovementVertical();
    }

    private void DoMovement()
    {
        Vector3 movement = new Vector3(inputHorizontal, 0f, inputVertical);

        rb.AddForce(movement * Acceleration);

        if (rb.velocity.magnitude > MaxSpeed)
        {
            rb.velocity = rb.velocity.normalized * MaxSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.StartsWith("Player"))
        {
            Vector3 dir = collision.contacts[0].point - transform.position;
            dir = -dir.normalized;
            dir.y = 0;
            GetComponent<Rigidbody>().AddForce(dir * CollisionForce);
            FindObjectOfType<AudioManager>().Play("Bump");
        }
    }

    private void HandleDeath()
    {
        NumLife--;
        UpdateLifeText();

        if (NumLife > 0)
        {
            transform.position = SpawnPoint;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            GameObject manager = GameObject.Find("IceGameManager");
            IceGameManager iceManager = (IceGameManager)manager.GetComponent(typeof(IceGameManager));
            iceManager.HandlePlayerDeath(this.name);
            Destroy(this.gameObject);
        }
    }

    private void UpdateLifeText()
    {
        lifeText.text = this.name + " Lives: " + NumLife.ToString();
    }

}
