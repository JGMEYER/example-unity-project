using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleOverTime : MonoBehaviour
{

    public float time = 3f;
    public float numToggles = 2f;

    private bool _started = false;
    private float _interval;
    private float _timer;

    void Start()
    {
        _timer = time;
        _interval = time / numToggles / 2;
    }

    void Update()
    {
        if (_started && _timer > 0f)
        {
            _timer -= Time.deltaTime;

            if (_timer < 0f)
            {
                _timer = 0f;
            }
        }
    }

    public void BeginToggle()
    {
        _started = true;
    }

    public bool IsFinished()
    {
        return _timer <= 0f;
    }

    public bool ToggleIsTrue()
    {
        return Mathf.Floor(_timer / _interval) % 2 == 0;
    }

}
