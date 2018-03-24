using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownItem : MonoBehaviour
{

    private float TargetX;
    private float DistanceZ;
    private float Amplitude;
    private float Speed;
    private float A;  // width modifier for parabola
    private float CurrentZ;

    private void Update()
    {
        CurrentZ -= Time.deltaTime * Speed;

        float y = -1 * A * Mathf.Pow(CurrentZ - (DistanceZ / 2), 2) + Amplitude;
        transform.position = new Vector3(TargetX, y, CurrentZ);
    }

    public void Initialize(float targetX, float distanceZ, float amplitude, float speed)
    {
        TargetX = targetX;
        DistanceZ = distanceZ;
        Amplitude = amplitude;
        Speed = speed;

        // solve for A using (0, 0, 0)
        A = -1 * (-1 * Amplitude / Mathf.Pow(-distanceZ / 2, 2));

        CurrentZ = DistanceZ;
    }

}
