using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSelect : MonoBehaviour
{

    private GlobalControls globalControls;

    private void Start()
    {
        globalControls = InputManager.Instance.GlobalControls();

        UpdateText();
    }

    private void Update()
    {
        if (globalControls.GetExitKeyDown())
        {
            // TODO string should be generalized somewhere
            SceneManager.LoadSceneAsync("GameSelect");
        }
    }

    private void OnEnable()
    {
        InputEventManager.Instance.StartListening(InputEvent.PlayerControlsAssigned, UpdateText);
    }

    private void OnDisable()
    {
        InputEventManager.Instance.StopListening(InputEvent.PlayerControlsAssigned, UpdateText);
    }

    // Hacky proof-of-concept demo.
    private void UpdateText()
    {
        foreach (PlayerNumber playerNumber in Enum.GetValues(typeof(PlayerNumber)))
        {
            Text playerText = GameObject.Find("Player" + (int)playerNumber + " Text").GetComponent<Text>();
            playerText.text = "Player" + (int)playerNumber + ": ";

            IPlayerControls playerControlsAssignment = InputManager.Instance.PlayerControlsAssignments[playerNumber];

            if (playerControlsAssignment == null)
            {
                playerText.text += "Join with: ";
                foreach (IPlayerControls playerControls in InputManager.Instance.AvailablePlayerControls)
                {
                    playerText.text += playerControls.GetJoinGameKeyName() + " ";
                }
            }
            else
            {
                playerText.text += "[Assigned " + playerControlsAssignment.GetType().ToString() + "]";
            }
        }
    } 

}
