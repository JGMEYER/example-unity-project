using UnityEngine;

public class EggCatchPlayer : Player
{

    [SerializeField]
    private EggCatchGameManager gameManager;

    private Rigidbody rb;
    private float inputHorizontal;
    private float inputVertical;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (active)
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
        float velocityX = inputHorizontal * Time.deltaTime * 400;
        float velocityZ = inputVertical * Time.deltaTime * 400;

        Vector3 newVelocity = new Vector3(velocityX, 0f, velocityZ);
        rb.velocity = newVelocity;
        rb.angularVelocity = Vector3.zero;  // stop player from spinning on collision

        if (inputHorizontal != 0 || inputVertical != 0)
        {
            float rotationY = (rb.velocity.sqrMagnitude > 0 ? Quaternion.LookRotation(rb.velocity, Vector3.up).eulerAngles.y : 0);
            transform.eulerAngles = new Vector3(0, rotationY + 180, 0);
        }

        inputHorizontal = 0;
        inputVertical = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        EggCatchEgg egg = other.GetComponent<EggCatchEgg>();
        if (egg)
        {
            gameManager.AddPoints(playerNumber, egg.PointValue);
            Destroy(egg.gameObject);
        }
    }

}
