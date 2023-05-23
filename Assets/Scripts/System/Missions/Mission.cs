using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mission : ScriptableObject
{
    public int idMission;
    public bool Done;
    public int Points;
    public string Title;
    public string Description;
    public GameEvent Event;
    public GameEvent<int, string, Color> changeStatus;
    public abstract void initValues();
    public abstract void execute(GameEvent gameEvent);

    public abstract void updateText();
}
public abstract class Mission<T> : ScriptableObject
{
    public int idMission;
    public bool Done;
    public int Points;
    public string Title;
    public string Description;
    public GameEvent<T> Event;
    public GameEvent<int, string, Color> changeStatus;
    public abstract GameEventListener<T> initValues(GameObject gameObject);
    public abstract void execute(GameEvent<T> gameEvent, T parameter);

    public abstract void updateText();
}
public abstract class Mission<T1, T2> : ScriptableObject
{
    public int idMission;
    public bool Done;
    public int Points;
    public string Title;
    public string Description;
    public GameEvent<T1,T2> Event;
    public GameEvent<int, string, Color> changeStatus;
    public abstract GameEventListener<T1,T2> initValues(GameObject gameObject);
    public abstract void execute(GameEvent<T1,T2> gameEvent, T1 parameter1,  T2 parameter2);
    public abstract void updateText();
}
public abstract class Mission<T1, T2, T3> : ScriptableObject
{
    public int idMission;
    public bool Done;
    public int Points;
    public string Title;
    public string Description;
    public GameEvent<T1,T2,T3> Event;
    public GameEvent<int, string, Color> changeStatus;
    public abstract GameEventListener<T1, T2,T3> initValues(GameObject gameObject);
    public abstract void execute(GameEvent<T1,T2,T3> gameEvent, T1 parameter1, T2 parameter2, T3 parameter3);
    public abstract void updateText();
}