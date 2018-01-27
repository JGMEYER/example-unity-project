using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class IcePlayer : MonoBehaviour {

    public float speed;
    public string inputKeys = "wasd";
    public Vector3 spawnPoint;
    public float collisionForce;
    public int numLife = 3;
    public Text lifeText;

    private Rigidbody rb;
    private string left;
    private string right;
    private string up;
    private string down;
    private Vector3 currentPosition;

    void Start() {
        rb = GetComponent<Rigidbody>();
        transform.position = spawnPoint;

        updateLifeText();
    }

    void FixedUpdate() {
        float moveHorizontal = 0;
        float moveVertical = 0;
        if (inputKeys.Equals("arrows")) {
            moveHorizontal = Input.GetAxis("Horizontal");
            moveVertical = Input.GetAxis("Vertical");
        } else if (inputKeys.Equals("wasd")) {
            moveHorizontal = Input.GetAxis("Horizontal2");
            moveVertical = Input.GetAxis("Vertical2");
        }
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * speed);
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.name.StartsWith("Player")) {
            Vector3 dir = collision.contacts[0].point - transform.position;
            dir = -dir.normalized;
            dir.y = 0;
            GetComponent<Rigidbody>().AddForce(dir * collisionForce);
        }
    }

    void Update() {
        currentPosition = transform.position;
        if (currentPosition.y < -20) {
            handleDeath();
        }
    }

    public Rigidbody GetRigidBody() {
        return rb;
    }

    private void handleDeath() {
        numLife--;
        updateLifeText();

        if (numLife > 0) {
            transform.position = spawnPoint;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        } else {
            GameObject manager = GameObject.Find("IceGameManager");
            IceGameManager iceManager = (IceGameManager)manager.GetComponent(typeof(IceGameManager));
            iceManager.handlePlayerDeath(this.name);
            Destroy(this);
        }
    }

    private void updateLifeText() {
        lifeText.text = this.name + " Lives: " + numLife.ToString();
    }
}
