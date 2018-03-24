using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryRigidBodies : MonoBehaviour
{

    public List<Rigidbody> RigidBodies = new List<Rigidbody>();

    private Vector3 lastPosition;
    new private Transform transform;

    private void Start()
    {
        transform = GetComponent<Transform>();
        lastPosition = transform.position;
    }

    private void LateUpdate()
    {
        foreach (Rigidbody rb in RigidBodies)
        {
            Vector3 velocity = (transform.position - lastPosition);
            rb.transform.Translate(velocity, transform);
        }

        lastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Collider>().GetComponent<Rigidbody>();
        if (rb != null)
        {
            Add(rb);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.GetComponent<Collider>().GetComponent<Rigidbody>();
        if (rb != null)
        {
            Remove(rb);
        }
    }

    private void Add(Rigidbody rb)
    {
        if (!RigidBodies.Contains(rb))
        {
            RigidBodies.Add(rb);
        }
    }

    private void Remove(Rigidbody rb)
    {
        if (RigidBodies.Contains(rb))
        {
            RigidBodies.Remove(rb);
        }
    }

}
