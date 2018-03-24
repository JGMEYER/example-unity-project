using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionSphere : MonoBehaviour
{

    public Color StartColor = Color.green;
    public Color EndColor = Color.red;
    public Color WaitColor = Color.yellow;

    private Material material;

    private void Start()
    {
        material = GetComponent<Renderer>().material;
        material.color = WaitColor;
    }

    public void SetAsStartColor()
    {
        material.color = StartColor;
    }

    public void SetAsEndColor()
    {
        material.color = EndColor;
    }

    public void SetAsWaitColor()
    {
        if (material != null)
        {
            material.color = WaitColor;
        }
    }

}
