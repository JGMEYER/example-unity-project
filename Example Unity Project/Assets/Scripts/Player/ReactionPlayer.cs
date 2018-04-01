using System.Collections;
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
    private bool isStunned;
    private Color playerColor;
    private Renderer playerRenderer;
    private int score;

    public float PlayerSpeed;
    public float stunTime;

    private int roundWins;

    private void Start()
    {
        startingPosition = transform.position;
        endingPosition = reactionToast.transform.position;
        isStunned = false;
        playerRenderer = this.gameObject.transform.GetChild(0).GetChild(4).gameObject.GetComponent<Renderer>();
        playerColor = playerRenderer.material.color;
        score = 0;
    }

    private void Update()
    {
        if (!isStunned) 
        { 
            HandleInput();
        }

        if (isMoving) {
            MoveToEnd();
        }
    }

    private void HandleInput()
    {
        if (controls.GetActionKeyDown())
        {
            float pressTime = Time.timeSinceLevelLoad;
            if (reactionGameManager.Grab(playerNumber, pressTime)) {
                isMoving = true;
            } else {
                Stun();
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

    public void Stun()
    {
        isStunned = true;
        playerRenderer.material.color = Color.red;
        StartCoroutine(WaitStunDuration());
    }


    private IEnumerator WaitStunDuration()
    {
        Debug.Log("Player " + playerNumber + " stunned for: " + stunTime);
        yield return new WaitForSeconds(stunTime);
        isStunned = false;
        playerRenderer.material.color = playerColor;
    }

    public void IncreaseScore()
    {
        score++;
    }

    public int GetScore()
    {
        return score;
    }
}
