using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class MissionsSystemManager : MonoBehaviour
{
    public int lvl;
    private PlayersPoints playersPoints;
    [SerializeField]
    private List<Mission> missions;
    private Dictionary<int, bool> missionsInitialValues = new Dictionary<int, bool>();
    public PlayerControlls playerControlls;
    private bool iker;
    protected virtual void Awake()
    {
        iker = false; 
        //playerControlls.EndLevelDelegator += checkIfWinPoints;
        List<GameEvent> listEvents = new List<GameEvent>();
        missions = Resources.LoadAll<Mission>("Missions/lvl" + lvl + "/MissionAsset/").ToList();
        playersPoints = Resources.Load<PlayersPoints>("Player/PlayersPoints");
        foreach (Mission mission in missions)
        {
            mission.initValues();
            if (!listEvents.Contains(mission.Event))
                listEvents.Add(mission.Event);
        }
        foreach (GameEvent evento in listEvents)
        {
            GameEventListener gameEventListener = gameObject.AddComponent<GameEventListener>() as GameEventListener;
            gameEventListener.Event = evento;
            gameEventListener.Response = new UnityEngine.Events.UnityEvent();
            gameEventListener.Response.AddListener(() => this.RaisedEvent(evento));
        }
        foreach (Mission mission in missions)
        {
            missionsInitialValues.Add(mission.idMission, mission.Done);
        }
    }
    public void checkIfWinPoints()
    {
        Debug.Log("check!!");
        foreach (Mission mission in missions)
        {
            bool b = missionsInitialValues.GetValueOrDefault(mission.idMission);
            if (!b && mission.Done)
            {
                playersPoints.Points += (int) mission.Points;
                playersPoints.DebugPoints();
            }
        }
    }

    public void RaisedEvent(GameEvent gameEvent)
    {
        foreach (Mission mission in missions)
            mission.execute(gameEvent);
    }

}

public class MissionsSystemManager<T> : MonoBehaviour
{
    private bool iker;
    public int lvl;
    private PlayersPoints playersPoints;
    [SerializeField]
    private List<Mission<T>> missions;
    private Dictionary<int, bool> missionsInitialValues = new Dictionary<int, bool>();
    public PlayerControlls playerControlls;
    protected virtual void Awake()
    {
        //playerControlls.EndLevelDelegator += checkIfWinPoints; 
        missions = Resources.LoadAll<Mission<T>>("Missions/lvl" + lvl + "/MissionAsset/").ToList();
        playersPoints = Resources.Load<PlayersPoints>("Player/PlayersPoints");
        foreach (Mission<T> mission in missions)
        {
            mission.initValues(this.gameObject).Response.AddListener(this.RaisedEvent);
        }
        foreach (Mission<T> mission in missions)
        {
            missionsInitialValues.Add(mission.idMission, mission.Done);
        }
    }
    public void checkIfWinPoints()
    {
        if (iker) return;
        iker = true;
        foreach (Mission<T> mission in missions)
        {
            bool b = missionsInitialValues.GetValueOrDefault(mission.idMission);
            if (!b && mission.Done)
            {
                playersPoints.Points += (int)mission.Points;
                playersPoints.DebugPoints();
            }
            else
            {
                Debug.Log(b + " " + mission.Done);
            }
        }
    }

    public void RaisedEvent(GameEvent<T> gameEvent, T parameter1)
    {
        foreach (Mission<T> mission in missions)
            mission.execute(gameEvent, parameter1); Debug.Log("aaaa");
    }
}
public class MissionsSystemManager<T1, T2> : MonoBehaviour
{
    public int lvl;
    private PlayersPoints playersPoints;
    [SerializeField]
    private List<Mission<T1, T2>> missions;
    private Dictionary<int, bool> missionsInitialValues = new Dictionary<int, bool>();
    public PlayerControlls playerControlls;
    protected virtual void Awake()
    {
        playerControlls.EndLevelDelegator += checkIfWinPoints;
        List<GameEvent<T1,T2>> listEvents = new List<GameEvent<T1,T2>>();
        missions = Resources.LoadAll<Mission<T1, T2>>("Missions/lvl" + lvl + "/MissionAsset/").ToList();
        playersPoints = Resources.Load<PlayersPoints>("Player/PlayersPoints");
        foreach (Mission<T1,T2> mission in missions)
        {
            mission.initValues(this.gameObject).Response.AddListener(this.RaisedEvent);
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
                playersPoints.Points += (int)mission.Points;
                playersPoints.DebugPoints();
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
    private PlayersPoints playersPoints;
    [SerializeField]
    private List<Mission<T1, T2, T3>> missions;
    private Dictionary<int, bool> missionsInitialValues = new Dictionary<int, bool>();
    public PlayerControlls playerControlls;
    protected virtual void Awake()
    {
        playerControlls.EndLevelDelegator += checkIfWinPoints;
        List<GameEvent<T1, T2, T3>> listEvents = new List<GameEvent<T1, T2, T3>>();
        missions = Resources.LoadAll<Mission<T1, T2, T3>>("Missions/lvl" + lvl + "/MissionAsset/").ToList();
        playersPoints = Resources.Load<PlayersPoints>("Player/PlayersPoints");
        foreach (Mission<T1, T2, T3> mission in missions)
        {
            mission.initValues(this.gameObject).Response.AddListener(this.RaisedEvent);
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
                playersPoints.Points += (int)mission.Points;
                playersPoints.DebugPoints();
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