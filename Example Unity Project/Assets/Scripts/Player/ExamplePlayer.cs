using UnityEngine;

public class ExamplePlayer : Player {

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

        inputHorizontal = 0;
        inputVertical = 0;
    }

}
