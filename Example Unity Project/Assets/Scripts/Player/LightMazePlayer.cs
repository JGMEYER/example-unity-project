using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMazePlayer : Player
{

    [SerializeField]
    private ParticleSystem deathExplosion;

    [Header("Movement")]
    public bool CanMove = true;
    public bool CanWallJump = false;
    public float HorizontalSpeed = 8f;
    public float InitialJumpVelocity = 3f;
    public float MaxJumpHoldTime = 0.8f;
    public float FallGravityMultiplier = 2.5f;

    [Header("Detectors")]
    public float RayCastDist = 0.27f;

    private Rigidbody rb;

    private float inputHorizontal = 0;
    private int inputVertical = 0;

    private float jumpHoldCounter = 0;
    private bool canJump = true;
    private bool isDead = false;

    private LightMazeJetpack jetpack = null;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isDead)
        {
            DoInput();
        }
    }

    void FixedUpdate()
    {
        if (CanMove)
        {
            if (HasJetpack())
            {
                DoJetpackMovement();
            }
            else
            {
                DoMovement();
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * RayCastDist);
        Gizmos.DrawRay(transform.position, Vector3.left * RayCastDist);
        Gizmos.DrawRay(transform.position, Vector3.right * RayCastDist);

        if (canJump)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.1f);
        }

        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, rb.velocity);
        }
    }

    void DoInput()
    {
        if (controls.GetJump())
        {
            jumpHoldCounter -= Time.deltaTime;
        }

        if (controls.GetJump() && canJump)
        {
            inputVertical = 1;
            jumpHoldCounter = MaxJumpHoldTime;
            canJump = false;
        }

        inputHorizontal = controls.GetMovementHorizontal();
    }

    void DoMovement()
    {
        Vector3 resetHorizontalVelocity = new Vector3(0, rb.velocity.y, 0);
        rb.velocity = resetHorizontalVelocity;

        rb.velocity += Vector3.up * inputVertical * InitialJumpVelocity * Time.deltaTime;

        if (rb.velocity.y < 0 || !controls.GetJump() || jumpHoldCounter <= 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (FallGravityMultiplier - 1) * Time.deltaTime;
        }

        rb.velocity += Vector3.right * inputHorizontal * HorizontalSpeed * Time.deltaTime;

        inputHorizontal = 0;
        inputVertical = 0;
    }

    void DoJetpackMovement()
    {
        // I am sure I will regret this design choice
        Vector3 jetpackVelocity = jetpack.GetVelocity(inputHorizontal);
        rb.velocity = jetpackVelocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        CheckCanJump(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        CheckCanJump(collision);
    }

    void OnCollisionExit(Collision collision)
    {
        if (!canJump)
        {
            return;
        }

        RaycastHit hit;

        bool collisionBelow = false;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            collisionBelow |= hit.distance <= RayCastDist;
        }

        canJump &= collisionBelow;
    }

    void CheckCanJump(Collision collision)
    {
        if (collision.gameObject.name.StartsWith("Player"))
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
            return;
        }

        RaycastHit hit;

        bool collisionBelow = false;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            collisionBelow |= hit.distance <= RayCastDist;
        }

        canJump |= collisionBelow;

        if (CanWallJump)
        {
            bool collisionLeft = false;
            if (Physics.Raycast(transform.position, Vector3.left, out hit))
            {
                collisionLeft |= hit.distance <= RayCastDist;
            }

            bool collisionRight = false;
            if (Physics.Raycast(transform.position, Vector3.right, out hit))
            {
                collisionRight |= hit.distance <= RayCastDist;
            }

            canJump |= (collisionLeft || collisionRight);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        LightMazeJetpack jetpackItem = other.GetComponent<LightMazeJetpack>();

        if (jetpackItem != null && !jetpackItem.IsEquipped())
        {
            EquipJetpack(jetpackItem);
        }
    }

    void EquipJetpack(LightMazeJetpack jetpackItem)
    {
        jetpack = jetpackItem;
        jetpack.SetEquipped(true);

        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.transform.rotation = Quaternion.Euler(Vector3.zero);

        // I am sure I will regret this design choice
        transform.position = jetpack.transform.position;
        jetpack.transform.parent = transform;
        jetpack.transform.localPosition = Vector3.zero;

        Vector3 newPosition = transform.position;
        newPosition.z = -3f; // arbitrary
        transform.position = newPosition;
    }

    public bool HasJetpack()
    {
        return (jetpack != null);
    }

    public void Kill(bool explode)
    {
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        isDead = true;
        CanMove = false;

        if (explode)
        {
            deathExplosion.Emit(5);
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

}
