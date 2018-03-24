using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMazeJetpack : MonoBehaviour
{

    [Header("Movement")]
    public float HorizontalSpeed = 3f;
    public float VerticalSpeed = 3f;

    private bool equipped = false;

    public Vector3 GetVelocity(float inputHorizontal)
    {
        return new Vector3(inputHorizontal * HorizontalSpeed, VerticalSpeed, 0);
    }

    public void SetEquipped(bool equipped)
    {
        this.equipped = equipped;
    }

    public bool IsEquipped()
    {
        return equipped;
    }

}
