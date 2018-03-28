using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{

    public GameObject TargetObject;
    public Vector3 OffsetFromObject;

    private void Update()
    {
        if (TargetObject != null)
        {
            Vector3 targetPosition = TargetObject.transform.position;
            transform.position = targetPosition + OffsetFromObject;
        }
    }

}
