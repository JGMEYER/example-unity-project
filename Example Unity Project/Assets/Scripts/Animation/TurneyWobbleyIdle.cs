using UnityEngine;

public class TurneyWobbleyIdle : IdleAnim
{

    public float turnSpeed = 2f;
    public float maxTurnAngle = 10f;
    public float wobbleSpeed = 1f;
    public float maxWobbleAngle = 2f;
    private float turnTimer = 0f;

    void Update()
    {
        turnTimer += Time.deltaTime;

        float turnY = Mathf.Sin(turnTimer * turnSpeed) * maxTurnAngle;
        float turnZ = Mathf.Sin(turnTimer * wobbleSpeed) * maxWobbleAngle;
        transform.eulerAngles = new Vector3(0, turnY, turnZ);
    }

    public new void Restart()
    {
        turnTimer = 0f;
    }

}
