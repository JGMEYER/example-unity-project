using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    protected PlayerNumber playerNumber;

    protected IPlayerControls controls { get; private set; }

    public void Awake()
    {
        controls = GameControlsManager.Instance.PlayerControls(playerNumber);
    }

    public void OnValidate()
    {
        if (playerNumber == 0)
        {
            throw new System.ArgumentException("Player is missing PlayerNumber assignment.");
        }
    }

}
