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
            else if(eventListeners[i].GetType() == typeof(GameEventListener))
            {
                GameEventListener mel = (GameEventListener)eventListeners[i];
                mel.OnEventRaised();
            }else if(eventListeners[i].GetType() == typeof(Tutorialer))
            {
                Tutorialer tut = (Tutorialer)eventListeners[i];
                tut.eventEncerrar1.Invoke(false);
            }
            else if (eventListeners[i].GetType() == typeof(Quiraxian))
            {
                Debug.Log("Hola");
                Quiraxian eb = (Quiraxian)eventListeners[i];
                eb.DestroySelf();

            }
            else if (eventListeners[i].GetType() == typeof(Thraaxian))
            {
                Debug.Log("Hola");
                Thraaxian eb = (Thraaxian)eventListeners[i];
                eb.DestroySelf();

            }
            else if (eventListeners[i].GetType() == typeof(Zorgonian))
            {
                Debug.Log("Hola");
                Zorgonian eb = (Zorgonian)eventListeners[i];
                eb.DestroySelf();

            }
            else
            {
                IMissionManager mission = (IMissionManager)eventListeners[i];
                mission.SetText();
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