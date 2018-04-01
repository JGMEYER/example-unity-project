using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSelect : MonoBehaviour
{

    [SerializeField]
    private GameObject uiPlayerKeyboardControlsLayoutPrefab;
    [SerializeField]
    private GameObject uiPlayerJoystickControlsLayoutPrefab;

    private GlobalControls globalControls;

    private void Start()
    {
        globalControls = InputManager.Instance.GlobalControls();
        InputManager.Instance.ClearPlayerControlsAssignments();

        UpdatePlayerSelectUI();
    }

    private void Update()
    {
        if (globalControls.GetExitKeyDown())
        {
            // TODO string should be generalized somewhere
            SceneManager.LoadSceneAsync("GameSelect");
        }
    }

    private void OnValidate()
    {
        if (uiPlayerKeyboardControlsLayoutPrefab && uiPlayerKeyboardControlsLayoutPrefab.GetComponent<PlayerKeyboardControlsLayout>() == null)
        {
            throw new MissingComponentException("uiPlayerKeyboardControlsLayoutPrefab requires a PlayerKeyboardControls script.");
        }
    }

    private void OnEnable()
    {
        InputEventManager.Instance.StartListening(InputEvent.PlayerPressedSubmit, AttemptSceneChange);
        InputEventManager.Instance.StartListening(InputEvent.PlayerControlsAssigned, UpdatePlayerSelectUI);
    }

    private void OnDisable()
    {
        InputEventManager.Instance.StopListening(InputEvent.PlayerPressedSubmit, AttemptSceneChange);
        InputEventManager.Instance.StopListening(InputEvent.PlayerControlsAssigned, UpdatePlayerSelectUI);
    }

    private void AttemptSceneChange()
    {
        if (InputManager.Instance.EnoughPlayersRegistered())
        {
            // TODO string should be generalized somewhere
            SceneManager.LoadScene("GameSelect");
        }
    }

    private void UpdatePlayerSelectUI()
    {
        Dictionary<PlayerNumber, IPlayerControls> playerControlsAssignments = InputManager.Instance.PlayerControlsAssignments;
        List<IPlayerControls> availablePlayerControls = InputManager.Instance.AvailablePlayerControls;

        foreach (PlayerNumber playerNumber in Enum.GetValues(typeof(PlayerNumber))) {
            UpdatePlayerPanel(playerNumber, playerControlsAssignments[playerNumber], availablePlayerControls);
        }
    }

    private List<IPlayerControls> GetAvailablePlayerControlsToDisplay(List<IPlayerControls> availablePlayerControls)
    {
        List<IPlayerControls> playerControlsToDisplay = new List<IPlayerControls>();
        bool joystickControlsAlreadyAdded = false;

        foreach (IPlayerControls playerControls in availablePlayerControls)
        {
            if (!(joystickControlsAlreadyAdded && playerControls is PlayerJoystickControls)) {
                playerControlsToDisplay.Add(playerControls);

                if (playerControls is PlayerJoystickControls)
                {
                    joystickControlsAlreadyAdded = true;
                }
            }
        }

        return playerControlsToDisplay;
    }

    private GameObject GetControlsLayout(IPlayerControls playerControls)
    {
        GameObject controlsLayout;

        if (playerControls is PlayerKeyboardControls)
        {
            GameObject keyboardControlsLayout = Instantiate(uiPlayerKeyboardControlsLayoutPrefab);
            keyboardControlsLayout.GetComponent<PlayerKeyboardControlsLayout>().Init((PlayerKeyboardControls)playerControls);
            controlsLayout = keyboardControlsLayout;
        }
        else if (playerControls is PlayerJoystickControls)
        {
            controlsLayout = Instantiate(uiPlayerJoystickControlsLayoutPrefab);
        }
        else
        {
            throw new ArgumentException("Unknown player controls type");
        }

        return controlsLayout;
    }

    // Hacky proof-of-concept demo
    private void UpdatePlayerPanel(PlayerNumber playerNumber, IPlayerControls playerControls, List<IPlayerControls> availablePlayerControls)
    {
        // Hacky: coupling with GameObject name
        GameObject playerPanel = GameObject.Find("Player" + (int)playerNumber + " Panel");
        Vector3 playerPanelPosition = playerPanel.transform.position;

        // Clear panel
        foreach (Transform child in playerPanel.transform)
        {
            Destroy(child.gameObject);
        }

        if (playerControls == null)
        {
            DisplayAvailableControls(playerPanel, availablePlayerControls);
        }
        else
        {
            DisplaySelectedPlayerControls(playerPanel, playerNumber, playerControls);
        }
    }

    private void DisplayAvailableControls(GameObject playerPanel, List<IPlayerControls> availablePlayerControls)
    {
        List<IPlayerControls> playerControlsToDisplay = GetAvailablePlayerControlsToDisplay(availablePlayerControls);

        // Hacky: assumes both prefabs same height
        float controlsLayoutHeight = uiPlayerKeyboardControlsLayoutPrefab.GetComponent<RectTransform>().rect.height;
        float controlsLayoutBuffer = 20f;

        // Starting Y position
        float controlsLayoutY = ((controlsLayoutHeight * playerControlsToDisplay.Count) + (controlsLayoutBuffer * (playerControlsToDisplay.Count - 1))) / 2 - controlsLayoutHeight / 2;

        foreach (IPlayerControls availableControls in playerControlsToDisplay)
        {
            GameObject controlsLayout = GetControlsLayout(availableControls);
            controlsLayout.transform.SetParent(playerPanel.transform);
            controlsLayout.transform.localPosition = new Vector3(0, controlsLayoutY, 0);
            controlsLayoutY -= controlsLayoutHeight + controlsLayoutBuffer;
        }
    }

    private void DisplaySelectedPlayerControls(GameObject playerPanel, PlayerNumber playerNumber, IPlayerControls playerControls)
    {
        GameObject textGameObject = new GameObject("Joined Text");
        Text joinedText = textGameObject.AddComponent<Text>();

        // Hacky: should be put in a prefab or something
        joinedText.text = "Player" + (int)playerNumber + " Joined";
        joinedText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        joinedText.alignment = TextAnchor.MiddleCenter;
        joinedText.color = Color.black;

        joinedText.transform.SetParent(playerPanel.transform);
        joinedText.transform.localPosition = Vector3.zero;

        // Hacky: assumes both prefabs same height
        float controlsLayoutHeight = uiPlayerKeyboardControlsLayoutPrefab.GetComponent<RectTransform>().rect.height;
        float controlsLayoutBuffer = 20f;

        GameObject controlsLayout = GetControlsLayout(playerControls);
        float controlsLayoutY = joinedText.transform.localPosition.y - joinedText.preferredHeight / 2 - controlsLayoutHeight / 2 - controlsLayoutBuffer;
        controlsLayout.transform.SetParent(playerPanel.transform);
        controlsLayout.transform.localPosition = new Vector3(0, controlsLayoutY, 0);
    }

}
