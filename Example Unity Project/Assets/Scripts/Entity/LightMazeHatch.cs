using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMazeHatch : MonoBehaviour
{

    public float smooth = 0.5f;

    [SerializeField]
    private GameObject hatch;
    [SerializeField]
    private GameObject lazer;

    bool _triggered = false;
    bool _activated = false;

    public void Update()
    {
        if (_triggered && !_activated)
        {
            Renderer lazerRenderer = lazer.GetComponent<Renderer>();
            ToggleOverTime tot = lazer.GetComponent<ToggleOverTime>();

            if (tot.IsFinished())
            {
                lazerRenderer.enabled = true;

                Vector3 hatchScale = hatch.transform.localScale;
                Vector3 activatedHatchScale = new Vector3(1, hatchScale.y, hatchScale.z);
                hatch.transform.localScale = activatedHatchScale;
                hatch.gameObject.SetActive(true);

                _activated = true;
            }
            else
            {
                lazerRenderer.enabled = tot.ToggleIsTrue();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ToggleOverTime tot = lazer.GetComponent<ToggleOverTime>();

        if (!_triggered)
        {
            tot.BeginToggle();
            _triggered = true;
        }
    }

}
