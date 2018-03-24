using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleOverTime : MonoBehaviour
{

    public float ToggleDuration = 3f;
    public float ToggleCount = 2f;

    private bool started = false;
    private float interval;
    private float timer;

    private void Start()
    {
        timer = ToggleDuration;
        interval = ToggleDuration / ToggleCount / 2;
    }

    private void Update()
    {
        if (started && timer > 0f)
        {
            timer -= Time.deltaTime;

            if (timer < 0f)
            {
                timer = 0f;
            }
        }
    }

    public void BeginToggle()
    {
        started = true;
    }

    public bool IsFinished()
    {
        return timer <= 0f;
    }

    public bool ToggleIsTrue()
    {
        return (int)Mathf.Floor(timer / interval) % 2 == 0;
    }

}
