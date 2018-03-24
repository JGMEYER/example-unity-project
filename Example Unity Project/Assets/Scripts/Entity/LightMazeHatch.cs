using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMazeHatch : MonoBehaviour
{

    [SerializeField]
    private GameObject hatch;
    [SerializeField]
    private GameObject lazer;

    private bool triggered = false;
    private bool activated = false;

    private void Update()
    {
        if (triggered && !activated)
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

                activated = true;
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

        if (!triggered)
        {
            tot.BeginToggle();
            triggered = true;
        }
    }

}
