using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class IcePlayer : MonoBehaviour {

    public KeyCode upKey;
	public KeyCode leftKey;
    public KeyCode downKey;
    public KeyCode rightKey;
    public float speed;
    public Vector3 spawnPoint;
    public float collisionForce;
    public int numLife = 3;
    public Text lifeText;

    private Rigidbody rb;
    private Vector3 currentPosition;

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
        }
    }

    void Update() {
        currentPosition = transform.position;
        if (currentPosition.y < -20) {
            HandleDeath();
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
        rb.AddForce(movement * speed);
    }

    public Rigidbody GetRigidBody() {
        return rb;
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
            Destroy(this);
        }
    }

    private void updateLifeText() {
        lifeText.text = this.name + " Lives: " + numLife.ToString();
    }
}
