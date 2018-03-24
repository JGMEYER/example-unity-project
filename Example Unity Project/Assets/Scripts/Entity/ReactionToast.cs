using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionToast : MonoBehaviour {

    private Vector3 startingPosition;
    private Vector3 endingPosition;
    private bool isMoving;
    public float ToastSpeed;
    public float MaxToastHeight;

	void Start () {
        startingPosition = transform.position;
        endingPosition = new Vector3(startingPosition.x, startingPosition.y + MaxToastHeight, startingPosition.z);
        isMoving = false;
	}

    private void Update()
    {
        if (isMoving) 
        {
            FlyToastFly();
        }
    }

    private void FlyToastFly()
    {
        float step = ToastSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, endingPosition, step);
    }

    private void DieToastDie()
    {
        transform.position = startingPosition;
    }

    public void flingToast()
    {
        isMoving = true;
    }

    public void resetToast()
    {
        isMoving = false;
        DieToastDie();
    }
}
