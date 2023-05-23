// ----------------------------------------------------------------------------
// Unite 2017 - Game Architecture with Scriptable Objects
// 
// Author: Ryan Hipple
// Date:   10/04/17
// ----------------------------------------------------------------------------
// Modifying for generic parameter use

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public abstract class GameEvent<T> : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<IEventListener> eventListeners =
        new List<IEventListener>();

    public void Raise(T parameter)
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
        {
            if (eventListeners[i].GetType() == typeof(GameEventListener<T>))
            {
                GameEventListener<T> ey = (GameEventListener<T>)eventListeners[i];
                ey.OnEventRaised(parameter);
            }else if(eventListeners[i].GetType() == typeof(Tutorialer))
            {
                Tutorialer ey = (Tutorialer)eventListeners[i];
                ey.eventEncerrar1.Invoke(false);
                EnemyType a = (EnemyType) Convert.ToInt32(parameter);
                if(a == EnemyType.THRAAXIAN)
                {
                    ey.ChangeState(StateScene.State8, null);
                }else if(a == EnemyType.QUIRAXIAN)
                {
                    ey.ChangeState(StateScene.State11, null);
                }else if(a == EnemyType.ZORGONIAN)
                {
                    ey.ChangeState(StateScene.State14, null);
                }
            }
            
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

public abstract class GameEvent<T0, T1> : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<GameEventListener<T0, T1>> eventListeners =
        new List<GameEventListener<T0, T1>>();

    public void Raise(T0 parameter0, T1 parameter1)
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
            eventListeners[i].OnEventRaised(parameter0, parameter1);
    }

    public void RegisterListener(GameEventListener<T0, T1> listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener<T0, T1> listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}

public abstract class GameEvent<T0, T1, T2> : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<IEventListener> eventListeners =
        new List<IEventListener>();

    public void Raise(T0 parameter0, T1 parameter1, T2 parameter2)
    {
        Debug.Log("Pasa1");
        for (int i = eventListeners.Count - 1; i >= 0; i--)
        {
            if (eventListeners[i].GetType() == typeof(GameEventListener<T0, T1, T2>))
            {
                GameEventListener<T0, T1, T2> List = (GameEventListener<T0, T1, T2>)eventListeners[i];
                List.OnEventRaised(parameter0, parameter1, parameter2);
            }else if(eventListeners[i].GetType() == typeof(UIPlayerControlls))
            {
                Debug.Log("Pasa2");
                UIPlayerControlls ui = (UIPlayerControlls)eventListeners[i];
                ui.UpdateMissions(System.Convert.ToInt32(parameter0), System.Convert.ToString(parameter1), (Color)System.Convert.ChangeType(parameter2, typeof(Color)));
            }
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

//public abstract class GameEvent<T0, T1, T2, T3> : ScriptableObject
//{
//    /// <summary>
//    /// The list of listeners that this event will notify if it is raised.
//    /// </summary>
//    private readonly List<GameEventListener<T0, T1, T2, T3>> eventListeners =
//        new List<GameEventListener<T0, T1, T2, T3>>();

//    public void Raise(T0 parameter0, T1 parameter1, T2 parameter2, T3 parameter3)
//    {
//        for (int i = eventListeners.Count - 1; i >= 0; i--)
//            eventListeners[i].OnEventRaised(parameter0, parameter1, parameter2, parameter3);
//    }

//    public void RegisterListener(GameEventListener<T0, T1, T2, T3> listener)
//    {
//        if (!eventListeners.Contains(listener))
//            eventListeners.Add(listener);
//    }

//    public void UnregisterListener(GameEventListener<T0, T1, T2, T3> listener)
//    {
//        if (eventListeners.Contains(listener))
//            eventListeners.Remove(listener);
//    }
//}

