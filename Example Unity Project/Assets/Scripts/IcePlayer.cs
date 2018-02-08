using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class IcePlayer : MonoBehaviour {

    public KeyCode upKey;
	public KeyCode leftKey;
    public KeyCode downKey;
    public KeyCode rightKey;
    public float acceleration;
    public Vector3 spawnPoint;
    public float collisionForce;
    public int numLife = 3;
    public Text lifeText;
    public float maxSpeed = 8;

    private Rigidbody rb;
    private Vector3 currentPosition;
    private int fallFloor = 20;
    private int floorHeight = 2;
    private int resetHeight = -20;

    private string SceneSelect = "MainMenu";

    void Start() {
        rb = GetComponent<Rigidbody>();
        transform.position = spawnPoint;

        updateLifeText();
    }

    void FixedUpdate() {
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.name.StartsWith("Player")) {
            Vector3 dir = collision.contacts[0].point - transform.position;
            dir = -dir.normalized;
            dir.y = 0;
            GetComponent<Rigidbody>().AddForce(dir * collisionForce);
            FindObjectOfType<AudioManager>().Play("Bump");
        }
    }

    void Update() {
        currentPosition = transform.position;
        if (currentPosition.y < resetHeight) {
            HandleDeath();
        }

        if (currentPosition.y > floorHeight) {
            HandleInput();
        }
    }

    public Rigidbody GetRigidBody() {
        return rb;
    }

    private void HandleInput() {

        if (Input.GetKey(KeyCode.Escape)) {
            SceneManager.LoadSceneAsync(SceneSelect);
        }

        float moveHorizontal = 0;
        float moveVertical = 0;

        if (Input.GetKey(upKey)) {
            moveVertical += 1;
        }
        if (Input.GetKey(leftKey)) {
            moveHorizontal -= 1;
        }
        if (Input.GetKey(downKey)) {
            moveVertical -= 1;
        }
        if (Input.GetKey(rightKey)) {
            moveHorizontal += 1;
        }

        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);

        rb.AddForce(movement * acceleration);

        if (rb.velocity.magnitude > maxSpeed) {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    private void HandleDeath() {
        numLife--;
        updateLifeText();

        if (numLife > 0) {
            transform.position = spawnPoint;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        } else {
            GameObject manager = GameObject.Find("IceGameManager");
            IceGameManager iceManager = (IceGameManager)manager.GetComponent(typeof(IceGameManager));
            iceManager.HandlePlayerDeath(this.name);
            Destroy(this.gameObject);
        }
    }

    private void updateLifeText() {
        lifeText.text = this.name + " Lives: " + numLife.ToString();
    }
}
