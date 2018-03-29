using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputEventManager : PersistentSingleton<InputEventManager> {

    // Keep singleton-only by disabling constructor
    protected InputEventManager() { }

    private Dictionary<InputEvent, TrackedUnityEvent> eventDictionary;

    private void Awake()
    {
        eventDictionary = new Dictionary<InputEvent, TrackedUnityEvent>();
    }

    private void Update()
    {
        PollPlayerInputForEvents();
    }

    public void StartListening(InputEvent inputEvent, UnityAction listener)
    {
        TrackedUnityEvent thisEvent = null;
        if (eventDictionary.TryGetValue(inputEvent, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new TrackedUnityEvent();
            thisEvent.AddListener(listener);
            eventDictionary.Add(inputEvent, thisEvent);
        }
    }

    public void StopListening(InputEvent inputEvent, UnityAction listener)
    {
        TrackedUnityEvent thisEvent = null;
        if (eventDictionary.TryGetValue(inputEvent, out thisEvent))
        {
            thisEvent.RemoveListener(listener);

            if (!thisEvent.HasListeners())
            {
                eventDictionary.Remove(inputEvent);
            }
        }
    }

    public void TriggerEvent(InputEvent inputEvent)
    {
        TrackedUnityEvent thisEvent = null;
        if (eventDictionary.TryGetValue(inputEvent, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }

    public bool ListeningOnEvent(InputEvent inputEvent)
    {
        TrackedUnityEvent thisEvent = null;
        return eventDictionary.TryGetValue(inputEvent, out thisEvent);
    }

    // ====================
    // Input Event Checks
    // ====================

    private void PollPlayerInputForEvents()
    {
        if (ListeningOnEvent(InputEvent.PlayerPressedSubmit) && PlayerPressedSubmit())
        {
            TriggerEvent(InputEvent.PlayerPressedSubmit);
        }

        if (ListeningOnEvent(InputEvent.PlayerControlsAssigned) && PlayerControlsAssigned())
        {
            TriggerEvent(InputEvent.PlayerControlsAssigned);
        }
    }

    private bool PlayerPressedSubmit()
    {
        foreach (PlayerNumber playerNumber in Enum.GetValues(typeof(PlayerNumber)))
        {
            IPlayerControls playerControls = InputManager.Instance.PlayerControlsAssignments[playerNumber];
            if (playerControls != null && playerControls.GetSubmitDown())
            {
                return true;
            }
        }

        return false;
    }

    private bool PlayerControlsAssigned()
    {
        foreach (IPlayerControls playerControls in InputManager.Instance.AvailablePlayerControls)
        {
            if (playerControls.GetJoinGameDown())
            {
                bool assignSuccessful = InputManager.Instance.AssignControlsToNextAvailablePlayer(playerControls);
                if (assignSuccessful)
                {
                    return true;
                }
            }
        }

        return false;
    }

}
