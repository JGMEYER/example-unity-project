using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionSphere : MonoBehaviour {

    private Material material;
    public Color startColor = Color.green;
    public Color endColor = Color.red;
    public Color waitColor = Color.yellow;

	void Start () {
        material = GetComponent < Renderer>().material;
        material.color = waitColor;
    }
	
	public void SetAsStartColor() {
        material.color = startColor;
    }

    public void SetAsEndColor() {
        material.color = endColor;
    }

    public void SetAsWaitColor() {
        if (material != null) {
            material.color = waitColor;
        }
    }
}
