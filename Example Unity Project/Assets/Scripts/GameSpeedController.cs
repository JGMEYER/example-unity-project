using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpeedController : MonoBehaviour
{

    public bool controlSpeed = true;
    [Range(0.25f, 4f)]
    public float timeScale = 1f;

    float _lastTimeScale;

    private void Start()
    {
        _lastTimeScale = timeScale;
    }

    void Update()
    {
        if (controlSpeed = true && _lastTimeScale != timeScale)
        {
            Time.timeScale = timeScale;
        }
    }

}
