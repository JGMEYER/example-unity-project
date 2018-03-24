using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMazePiston : MonoBehaviour
{

    public float Smooth = 1f;
    public float MinHeight = 0f;
    public float MaxHeight = 1f;

    private Vector3 minScale;
    private Vector3 maxScale;
    private Vector3 targetScale;

    private void Start()
    {
        Vector3 startScale = transform.localScale;

        // Cylinder scales are weird
        minScale = new Vector3(startScale.x, MinHeight / 2, startScale.z);
        maxScale = new Vector3(startScale.x, MaxHeight / 2, startScale.z);

        transform.localScale = maxScale;
    }

    private void Update()
    {
        CheckTarget();
        Vector3 newScale = Vector3.Lerp(transform.localScale, targetScale, Smooth * Time.deltaTime);
        transform.position += newScale - transform.localScale;
        transform.localScale = newScale;
    }

    private void CheckTarget()
    {
        if (transform.localScale == minScale)
        {
            targetScale = maxScale;
        }
        else if (transform.localScale == maxScale)
        {
            targetScale = minScale;
        }
    }

}
