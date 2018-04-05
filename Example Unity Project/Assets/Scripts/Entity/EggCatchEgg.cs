using UnityEngine;

public class EggCatchEgg : MonoBehaviour
{

    public float FallSpeed = 0.1f;
    public int PointValue = 1;

    public void Update()
    {
        if (transform.position.y + transform.localScale.y < 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        DoMovement();
    }

    private void DoMovement()
    {
        Vector3 pos = transform.position;
        pos.y -= FallSpeed;

        transform.position = pos;
    }

}
