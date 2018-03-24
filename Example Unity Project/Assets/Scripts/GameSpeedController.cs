using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpeedController : MonoBehaviour
{

    public bool ControlSpeed = true;
    [Range(0.25f, 4f)]
    public float TimeScale = 1f;

    void OnValidate()
    {
        if (ControlSpeed)
        {
            Time.timeScale = TimeScale;
        }
    }

}
