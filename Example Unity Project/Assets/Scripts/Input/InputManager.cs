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
        UpdateAvailablePlayerControls();
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
        UpdateAvailablePlayerControls();
    }

    public bool AssignControlsToNextAvailablePlayer(IPlayerControls playerControls)
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

    public bool EnoughPlayersRegistered()
    {
        return PlayerControlsAssignments[PlayerNumber.One] != null && PlayerControlsAssignments[PlayerNumber.Two] != null;
    }

    public int NumPlayersRegistered()
    {
        int numPlayersRegistered = 0;

        // Possibly a better check than just looking for a count of non-null assignments.
        // Players should only be assigned consecutively, starting at PlayerNumber.ONE.
        foreach (PlayerNumber playerNumber in Enum.GetValues(typeof(PlayerNumber)))
        {
            if (PlayerControls(playerNumber) != null)
            {
                numPlayersRegistered++;
            }
            else
            {
                return numPlayersRegistered;
            }
        }

        return numPlayersRegistered;
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
        if (!EnoughPlayersRegistered())
        {
            Debug.LogWarning("Not enough players registered. Using default " +
                "control assignments. This could mean there is a problem " + 
                "with the player control assignment workflow OR you are " + 
                "running your game scene directly from the editor without " +
                "setting controls first.");
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

}
