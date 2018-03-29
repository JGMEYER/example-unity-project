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
            // Hacky: assumes both prefabs same height
            float controlsLayoutHeight = uiPlayerKeyboardControlsLayoutPrefab.GetComponent<RectTransform>().rect.height;
            float controlsLayoutBuffer = 20f;

            // Starting Y position
            float controlsLayoutY = ((controlsLayoutHeight * availablePlayerControls.Count) + (controlsLayoutBuffer * (availablePlayerControls.Count - 1))) / 2 - controlsLayoutHeight / 2;

            foreach (IPlayerControls availableControls in availablePlayerControls)
            {
                GameObject controlsLayout;

                if (availableControls is PlayerKeyboardControls)
                {
                    PlayerKeyboardControls playerKeyboardControls = (PlayerKeyboardControls)availableControls;
                    controlsLayout = GetUIKeyboardControlsLayout(playerKeyboardControls);
                }
                else if (availableControls is PlayerJoystickControls)
                {
                    controlsLayout = Instantiate(uiPlayerJoystickControlsLayoutPrefab);
                }
                else
                {
                    throw new ArgumentException("Unknown player controls type.");
                }

                controlsLayout.transform.SetParent(playerPanel.transform);
                controlsLayout.transform.localPosition = new Vector3(0, controlsLayoutY, 0);

                controlsLayoutY -= controlsLayoutHeight + controlsLayoutBuffer;
            }
        }
        else
        {
            GameObject textGameObject = new GameObject("Joined Text");
            Text joinedText = textGameObject.AddComponent<Text>();

            // Hacky: should be put in a prefab or something
            joinedText.text = "Player" + (int)playerNumber + " Joined\n" + playerControls.GetType().ToString();
            joinedText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            joinedText.alignment = TextAnchor.MiddleCenter;
            joinedText.color = Color.black;

            joinedText.transform.SetParent(playerPanel.transform);
            joinedText.transform.localPosition = Vector3.zero;
        }
    }

    private GameObject GetUIKeyboardControlsLayout(PlayerKeyboardControls playerKeyboardControls)
    {
        // Hacky: grabs Panel by its Image
        GameObject keyboardControlsLayout = Instantiate(uiPlayerKeyboardControlsLayoutPrefab);
        Image[] uiKeys = keyboardControlsLayout.GetComponentsInChildren<Image>();

        foreach (Image uiKey in uiKeys)
        {
            Text uiKeyText = uiKey.GetComponentInChildren<Text>();

            // Hacky: key names should be enums or standardized in some way
            switch(uiKey.name)
            {
                case "SubmitKey":
                    KeyCode submitKey = playerKeyboardControls.SubmitKey;
                    uiKeyText.text = KeyCodeCharacter.For(submitKey).ToString();
                    break;
                case "UpKey":
                    KeyCode upKey = playerKeyboardControls.UpKey;
                    uiKeyText.text = KeyCodeCharacter.For(upKey).ToString();
                    break;
                case "LeftKey":
                    KeyCode leftKey = playerKeyboardControls.LeftKey;
                    uiKeyText.text = KeyCodeCharacter.For(leftKey).ToString();
                    break;
                case "DownKey":
                    KeyCode downKey = playerKeyboardControls.DownKey;
                    uiKeyText.text = KeyCodeCharacter.For(downKey).ToString();
                    break;
                case "RightKey":
                    KeyCode rightKey = playerKeyboardControls.RightKey;
                    uiKeyText.text = KeyCodeCharacter.For(rightKey).ToString();
                    break;
            }
        }

        return keyboardControlsLayout;
    }

}
