using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
public class MissionsSystemManager : MonoBehaviour
{
    public int lvl;
    [SerializeField]
    private List<Mission> missions;
    private Dictionary<int, bool> missionsInitialValues = new Dictionary<int, bool>();
    protected virtual void Awake()
    {
        missions = Resources.LoadAll<Mission>("Missions/lvl" + lvl + "/MissionAsset/").ToList();
        foreach (Mission mission in missions)
        {
            mission.initValues();
        }
        foreach (Mission mission in missions)
        {
            Debug.Log("aaa");
            missionsInitialValues.Add(mission.idMission, mission.Done);
        }
    }
    public void checkIfWinPoints()
    {
        foreach (Mission mission in missions)
        {
            bool b = missionsInitialValues.GetValueOrDefault(mission.idMission);
            if (!b && mission.Done)
            {
                //Activar Evento para ganar puntos
            }
        }
    }

    public void RaisedEvent(GameEvent gameEvent)
    {
        Debug.Log("AAAAAAAAAAAAAAA");
        foreach(Mission mission in missions)
            mission.execute(gameEvent);
    }
    
}

public class MissionsSystemManager<T> : MonoBehaviour
{
    public int lvl;
    [SerializeField]
    private List<Mission<T>> missions;
    private Dictionary<int, bool> missionsInitialValues = new Dictionary<int, bool>();
    protected virtual void Awake()
    {
        missions = Resources.LoadAll<Mission<T>>("Missions/lvl" + lvl + "/MissionAsset/").ToList();
        foreach (Mission<T> mission in missions)
        {
            mission.initValues();
        }
        foreach (Mission<T> mission in missions)
        {
            missionsInitialValues.Add(mission.idMission, mission.Done);
        }
    }
    public void checkIfWinPoints()
    {
        foreach (Mission<T> mission in missions)
        {
            bool b = missionsInitialValues.GetValueOrDefault(mission.idMission);
            if (!b && mission.Done)
            {
                //Activar Evento para ganar puntos
            }
        }
    }
    
    public void RaisedEvent(GameEvent<T> gameEvent, T parameter1)
    {
        foreach (Mission<T> mission in missions)
            mission.execute(gameEvent, parameter1);
    }
}
public class MissionsSystemManager<T1, T2> : MonoBehaviour
{
    public int lvl;
    [SerializeField]
    private List<Mission<T1,T2>> missions;
    private Dictionary<int, bool> missionsInitialValues = new Dictionary<int, bool>();
    protected virtual void Awake()
    {
        missions = Resources.LoadAll< Mission <T1,T2>> ("Missions/lvl" + lvl + "/MissionAsset/").ToList();
        foreach (Mission<T1, T2> mission in missions)
        {
            mission.initValues();
        }
        foreach (Mission<T1, T2> mission in missions)
        {
            missionsInitialValues.Add(mission.idMission, mission.Done);
        }
    }
    public void checkIfWinPoints()
    {
        foreach (Mission<T1, T2> mission in missions)
        {
            bool b = missionsInitialValues.GetValueOrDefault(mission.idMission);
            if (!b && mission.Done)
            {
                //Activar Evento para ganar puntos
            }
        }
    }

    public void RaisedEvent(GameEvent<T1, T2> gameEvent, T1 parameter1, T2 parameter2)
    {
        foreach (Mission<T1, T2> mission in missions)
            mission.execute(gameEvent, parameter1, parameter2);
    }
}
public class MissionsSystemManager<T1, T2, T3> : MonoBehaviour
{
    public int lvl;
    [SerializeField]
    private List<Mission<T1, T2, T3>> missions;
    private Dictionary<int, bool> missionsInitialValues = new Dictionary<int, bool>();
    protected virtual void Awake()
    {
        missions = Resources.LoadAll<Mission<T1, T2, T3>>("Missions/lvl" + lvl + "/MissionAsset/").ToList();
        foreach (Mission<T1, T2, T3> mission in missions)
        {
            mission.initValues();
        }
            foreach (Mission<T1, T2, T3> mission in missions)
        {
            missionsInitialValues.Add(mission.idMission, mission.Done);
        }
    }
    public void checkIfWinPoints()
    {
        foreach (Mission<T1, T2, T3> mission in missions)
        {
            bool b = missionsInitialValues.GetValueOrDefault(mission.idMission);
            if (!b && mission.Done)
            {
                //Activar Evento para ganar puntos
            }
        }
    }

    public void RaisedEvent(GameEvent<T1, T2, T3> gameEvent, T1 parameter1, T2 parameter2, T3 parameter3)
    {
        foreach (Mission<T1, T2, T3> mission in missions)
            mission.execute(gameEvent, parameter1, parameter2, parameter3);
    }
}

//public class MissionsSystemManager<T1, T2, T3, T4> : MonoBehaviour
//{
//    public int lvl;
//    private List<Mission<T1, T2, T3, T4>> missions;
//    private Dictionary<int, bool> missionsInitialValues;
//    void Awake()
//    {
//        missions = Resources.LoadAll<Mission<T1, T2, T3, T4>>("missions/lvl" + lvl + "/").ToList();
//        foreach (Mission<T1, T2, T3, T4> mission in missions)
//        {
//            missionsInitialValues.Add(mission.idMission, mission.Done);
//        }
//    }
//    public void checkIfWinPoints()
//    {
//        foreach (Mission<T1, T2, T3, T4> mission in missions)
//        {
//            bool b = missionsInitialValues.GetValueOrDefault(mission.idMission);
//            if (!b && mission.Done)
//            {
//                //Activar Evento para ganar puntos
//            }
//        }
//    }

//    public void RaisedEvent(GameEvent<T1, T2, T3, T4> gameEvent, T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4)
//    {
//        foreach (Mission<T1, T2, T3, T4> mission in missions)
//            mission.execute(gameEvent, parameter1, parameter2, parameter3, parameter4);
//    }
//}