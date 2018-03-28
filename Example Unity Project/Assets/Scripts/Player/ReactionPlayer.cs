using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionPlayer : Player
{

    [SerializeField]
    private ReactionGameManager reactionGameManager;
    [SerializeField]
    private ReactionToast reactionToast;

    private Vector3 startingPosition;
    private Vector3 endingPosition;
    private bool isMoving;
    public float PlayerSpeed;

    private int roundWins;

    private void Start()
    {
        startingPosition = transform.position;
        endingPosition = reactionToast.transform.position;
    }

    private void Update()
    {
        HandleInput();
        if (isMoving) 
        {
            MoveToEnd();
        }
    }

    private void HandleInput()
    {
        if (controls.GetUpKeyDown())
        {
            float pressTime = Time.timeSinceLevelLoad;
            if (reactionGameManager.Grab(playerNumber, pressTime)) {
                isMoving = true;
            }
        }
    }

    private void MoveToEnd()
    {
        float step = PlayerSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, reactionToast.transform.position, step);
    }

    public void ResetPlayerPosition()
    {
        transform.position = startingPosition;
        isMoving = false;
    }

}
