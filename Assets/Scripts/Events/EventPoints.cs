using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName ="Player/Win Points Event", fileName = "WinPointsEvent")]
public class EventPoints : ScriptableObject
{
    private readonly List<WinPointsListener> eventListeners =
       new List<WinPointsListener>();

    public void Raise(int parameter)
    {
        Debug.Log(eventListeners.Count);
        for (int i = eventListeners.Count - 1; i >= 0; i--)
            eventListeners[i].OnEventRaised(parameter);

    }

    public void RegisterListener(WinPointsListener listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(WinPointsListener listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }

}
