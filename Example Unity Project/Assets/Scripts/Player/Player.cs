using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    protected PlayerNumber playerNumber;

    protected IPlayerControls controls { get; private set; }

    protected void Awake()
    {
        controls = InputManager.Instance.PlayerControls(playerNumber);
    }

    protected void OnValidate()
    {
        if (playerNumber == 0)
        {
            throw new System.ArgumentException("Player is missing PlayerNumber assignment.");
        }
    }

    public PlayerNumber GetPlayerNumber()
    {
        // Could be a public { get; protected set;} instead, if we add a custom inspector.
        return playerNumber;
    }

}
