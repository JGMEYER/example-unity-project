using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrackedUnityEvent : UnityEvent {

    private int nonPersistentListenersCount;

    public TrackedUnityEvent()
    {
        nonPersistentListenersCount = 0;
    }

    new public void AddListener(UnityAction call)
    {
        base.AddListener(call);
        nonPersistentListenersCount++;
    }

    new public void RemoveListener(UnityAction call)
    {
        base.RemoveListener(call);
        nonPersistentListenersCount--;
    }

    public bool HasListeners()
    {
        return nonPersistentListenersCount > 0;
    }

}
