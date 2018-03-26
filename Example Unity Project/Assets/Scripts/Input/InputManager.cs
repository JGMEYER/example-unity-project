using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : PersistentSingleton<InputManager>
{

    // Keep singleton-only by disabling constructor
    protected InputManager() { }

    public List<IPlayerControls> AvailablePlayerControls { get; private set; }
    public Dictionary<PlayerNumber, IPlayerControls> PlayerControlsAssignments { get; private set; }

    private void Awake()
    {
        ClearPlayerControlsAssignments();
        UpdateAvailablePlayerControls();  // inits availablePlayerControls
    }

    private void Update()
    {
        // Ideally we should only call this when Input.JoyStickNames() changes or when assignments change
        UpdateAvailablePlayerControls();

        PollPlayerInputForEvents();
    }

    // =============================
    // Player Controls Assignments
    // =============================

    public void ClearPlayerControlsAssignments()
    {
        PlayerControlsAssignments = new Dictionary<PlayerNumber, IPlayerControls>
        {
            { PlayerNumber.One, null },
            { PlayerNumber.Two, null },
            { PlayerNumber.Three, null },
            { PlayerNumber.Four, null },
        };
    }

    private void SetDefaultPlayerControlsAssignments()
    {
        PlayerControlsAssignments = new Dictionary<PlayerNumber, IPlayerControls>
        {
            { PlayerNumber.One, PlayerKeyboardControls(KeyboardConfigNumber.One) },
            { PlayerNumber.Two, PlayerKeyboardControls(KeyboardConfigNumber.Two) },
            { PlayerNumber.Three, PlayerKeyboardControls(KeyboardConfigNumber.Three) },
            { PlayerNumber.Four, PlayerKeyboardControls(KeyboardConfigNumber.Four) },
        };
    }

    private bool AssignControlsToNextAvailablePlayer(IPlayerControls playerControls)
    {
        bool assigned = false;

        foreach (PlayerNumber playerNumber in Enum.GetValues(typeof(PlayerNumber)))
        {
            if (PlayerControlsAssignments[playerNumber] == null)
            {
                PlayerControlsAssignments[playerNumber] = playerControls;
                assigned = true;

                break;
            }
        }

        if (assigned)
        {
            UpdateAvailablePlayerControls();
        }

        return assigned;
    }

    private bool NotEnoughPlayersRegistered()
    {
        return PlayerControlsAssignments[PlayerNumber.One] == null && PlayerControlsAssignments[PlayerNumber.Two] == null;
    }

    private void UpdateAvailablePlayerControls()
    {
        AvailablePlayerControls = new List<IPlayerControls>();

        List<PlayerKeyboardControls> claimedKeyboardControls = new List<PlayerKeyboardControls>();
        List<PlayerJoystickControls> claimedJoystickControls = new List<PlayerJoystickControls>();

        // Find already claimed controls
        foreach (PlayerNumber playerNumber in Enum.GetValues(typeof(PlayerNumber)))
        {
            IPlayerControls controlsAssignment = PlayerControlsAssignments[playerNumber];
            if (controlsAssignment != null)
            {
                if (controlsAssignment is PlayerKeyboardControls)
                {
                    claimedKeyboardControls.Add((PlayerKeyboardControls)controlsAssignment);
                }
                else if (controlsAssignment is PlayerJoystickControls)
                {
                    claimedJoystickControls.Add((PlayerJoystickControls)controlsAssignment);
                }
                else
                {
                    throw new ArgumentException("Unknown player controls type.");
                }
            }
        }

        // Add unclaimed keyboard controls
        foreach (KeyboardConfigNumber keyboardConfigNumber in Enum.GetValues(typeof(KeyboardConfigNumber)))
        {
            PlayerKeyboardControls matchingClaimedKeyboard = claimedKeyboardControls.SingleOrDefault(keyboardControls => keyboardControls.KeyboardConfigNumber == keyboardConfigNumber);

            if (matchingClaimedKeyboard == null)
            {
                AvailablePlayerControls.Add(PlayerKeyboardControls(keyboardConfigNumber));
            }
        }

        // Add unclaimed joystick controls
        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            int joystickNumber = i + 1;
            PlayerJoystickControls matchingClaimedJoystick = claimedJoystickControls.SingleOrDefault(joystickControls => joystickControls.JoystickNumber == joystickNumber);

            if (matchingClaimedJoystick == null)
            {
                AvailablePlayerControls.Add(PlayerJoystickControls(joystickNumber));
            }
        }
    }

    // =================
    // Global Controls
    // =================

    public GlobalControls GlobalControls()
    {
        return KeyboardInputStore.Instance.GlobalControls();
    }

    // =================
    // Player Controls
    // =================

    public IPlayerControls PlayerControls(PlayerNumber playerNumber)
    {
        if (NotEnoughPlayersRegistered() && Application.isEditor)
        {
            Debug.LogWarning("Not enough players registered. Using default " +
                "control assignments while in the editor. This could be " +
                "indicative of a problem with the player control assignment " +
                "workflow.");
            SetDefaultPlayerControlsAssignments();
        }

        return PlayerControlsAssignments[playerNumber];
    }

    private IPlayerControls PlayerKeyboardControls(KeyboardConfigNumber keyboardConfigNumber)
    {
        return KeyboardInputStore.Instance.PlayerKeyboardControls(keyboardConfigNumber);
    }

    private IPlayerControls PlayerJoystickControls(int joystickNumber)
    {
        return new PlayerJoystickControls(joystickNumber);
    }

    // ====================
    // Input Event Checks
    // ====================

    public void PollPlayerInputForEvents()
    {
        // Poll player input only if someone's requesting it
        if (InputEventManager.Instance.ListeningOnEvent(InputEvent.PlayerControlsAssigned))
        {
            foreach (IPlayerControls playerControls in AvailablePlayerControls)
            {
                if (playerControls.GetJoinGameDown())
                {
                    if (AssignControlsToNextAvailablePlayer(playerControls))
                    {
                        InputEventManager.Instance.TriggerEvent(InputEvent.PlayerControlsAssigned);
                    }
                }
            }
        }
    }

}
