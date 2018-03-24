using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryRigidBodies : MonoBehaviour
{

    public List<Rigidbody> RigidBodies = new List<Rigidbody>();

    private Vector3 lastPosition;
    new private Transform transform;

    void Start()
    {
        transform = GetComponent<Transform>();
        lastPosition = transform.position;
    }

    void LateUpdate()
    {
        foreach (Rigidbody rb in RigidBodies)
        {
            Vector3 velocity = (transform.position - lastPosition);
            rb.transform.Translate(velocity, transform);
        }

        lastPosition = transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Collider>().GetComponent<Rigidbody>();
        if (rb != null)
        {
            Add(rb);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.GetComponent<Collider>().GetComponent<Rigidbody>();
        if (rb != null)
        {
            Remove(rb);
        }
    }

    void Add(Rigidbody rb)
    {
        if (!RigidBodies.Contains(rb))
        {
            RigidBodies.Add(rb);
        }
    }

    void Remove(Rigidbody rb)
    {
        if (RigidBodies.Contains(rb))
        {
            RigidBodies.Remove(rb);
        }
    }

}
