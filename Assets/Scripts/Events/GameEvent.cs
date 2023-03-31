using System.Collections.Generic;
using UnityEngine;
public  class GameEvent : ScriptableObject
{
    private readonly List<IEventListener> eventListeners =
        new List<IEventListener>();

    public void Raise()
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
            if (eventListeners[i].GetType() == typeof(MissionEventListener))
            {
                MissionEventListener mel = (MissionEventListener)eventListeners[i];
                mel.OnEventRaised();
            }
            else
            {
                GameEventListener mel = (GameEventListener)eventListeners[i];
                mel.OnEventRaised();
            }
    }

    public void RegisterListener(IEventListener listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(IEventListener listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}