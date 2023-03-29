using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mission : ScriptableObject
{
    public int idMission;
    public bool Done;
    public float Points;
    public string Title;
    public string Description;
    public abstract void initValues();
    public abstract void execute(GameEvent gameEvent);
}
public abstract class Mission<T> : ScriptableObject
{
    public int idMission;
    public bool Done;
    public float Points;
    public string Title;
    public string Description;
    public abstract void initValues();
    public abstract void execute(GameEvent<T> gameEvent, T parameter);
}
public abstract class Mission<T1, T2> : ScriptableObject
{
    public int idMission;
    public bool Done;
    public float Points;
    public string Title;
    public string Description;
    public abstract void initValues();
    public abstract void execute(GameEvent<T1,T2> gameEvent, T1 parameter1,  T2 parameter2);
}
public abstract class Mission<T1, T2, T3> : ScriptableObject
{
    public int idMission;
    public bool Done;
    public float Points;
    public string Title;
    public string Description;
    public abstract void initValues();
    public abstract void execute(GameEvent<T1,T2,T3> gameEvent, T1 parameter1, T2 parameter2, T3 parameter3);
}
//public abstract class Mission<T1, T2, T3, T4> : ScriptableObject
//{
//    public int idMission;
//    public bool Done;
//    public float Points;
//    public string Title;
//    public string Description;
//    public abstract void initValues();
//    public abstract void execute(GameEvent<T1, T2, T3,T4> gameEvent, T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4);
//}