using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMazeJetpack : MonoBehaviour
{

    [Header("Inputs")]
    public KeyCode upKey;
    public KeyCode leftKey;
    public KeyCode rightKey;

    [Header("Movement")]
    public float translationZ = -3f;
    public float horizontalSpeed = 3f;
    public float verticalSpeed = 3f;

    private bool _equipped = false;

    public Vector3 GetVelocity(float inputHorizontal)
    {
        return new Vector3(inputHorizontal * horizontalSpeed, verticalSpeed, 0);
    }

    public void SetEquipped(bool equipped)
    {
        _equipped = equipped;
    }

    public bool IsEquipped()
    {
        return _equipped;
    }

}
