using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionsSystemManager : MonoBehaviour
{
    public int lvl;
    [SerializeField]
    private List<Mission> missions;
    private Dictionary<int, bool> missionsInitialValues = new Dictionary<int, bool>();
    private bool iker;
    protected virtual void Awake()
    {
        if (!NetworkManager.Singleton.IsServer) { 
            Destroy(gameObject); 
            return;
        }
        iker = false; 
        
        List<GameEvent> listEvents = new List<GameEvent>();
        missions = Resources.LoadAll<Mission>("Missions/lvl" + lvl + "/MissionAsset/").ToList();
        
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
    private void OnTriggerEnter(Collider other)
    {
        PlayerControlls PC = other.gameObject.transform.parent.gameObject.GetComponent<PlayerControlls>();
        NetworkObject no = other.gameObject.transform.parent.gameObject.GetComponent<NetworkObject>();
        if (PC == null || !no.IsOwner) return;

        foreach (Mission mission in missions)
        {
            bool b = missionsInitialValues.GetValueOrDefault(mission.idMission);
            if (!b && mission.Done)
            {
                PC.WinPoints(mission.Points);
                missionsInitialValues[mission.idMission] = mission.Done;
            }
        }
        SaveGameManager.Singleton.saveGame.LevelsCompleted++;
        SaveGameManager.Singleton.SaveClientRpc();
        GetComponent<BoxCollider>().enabled = false;
        NetworkManager.Singleton.SceneManager.LoadScene("LevelMenu", LoadSceneMode.Single);
    }

    public void RaisedEvent(GameEvent gameEvent)
    {
        foreach (Mission mission in missions)
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
        if (!NetworkManager.Singleton.IsServer)
        {
            Destroy(gameObject);
            return;
        }
        missions = Resources.LoadAll<Mission<T>>("Missions/lvl" + lvl + "/MissionAsset/").ToList();

        foreach (Mission<T> mission in missions)
        {
            mission.initValues(this.gameObject).Response.AddListener(this.RaisedEvent);
        }

        foreach (Mission<T> mission in missions)
        {
            missionsInitialValues.Add(mission.idMission, mission.Done);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerControlls PC = other.gameObject.GetComponent<PlayerControlls>();
        if (PC == null) return;

        foreach (Mission<T> mission in missions)
        {
            bool b = missionsInitialValues.GetValueOrDefault(mission.idMission);
            if (!b && mission.Done)
            {
                PC.WinPoints(mission.Points);
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
    private List<Mission<T1, T2>> missions;
    private Dictionary<int, bool> missionsInitialValues = new Dictionary<int, bool>();
    protected virtual void Awake()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Destroy(gameObject);
            return;
        }
        List<GameEvent<T1,T2>> listEvents = new List<GameEvent<T1,T2>>();
        missions = Resources.LoadAll<Mission<T1, T2>>("Missions/lvl" + lvl + "/MissionAsset/").ToList();

        foreach (Mission<T1,T2> mission in missions)
        {
            mission.initValues(this.gameObject).Response.AddListener(this.RaisedEvent);
        }
        foreach (Mission<T1, T2> mission in missions)
        {
            missionsInitialValues.Add(mission.idMission, mission.Done);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerControlls PC = other.gameObject.GetComponent<PlayerControlls>();
        if (PC == null) return;

        foreach (Mission<T1, T2> mission in missions)
        {
            bool b = missionsInitialValues.GetValueOrDefault(mission.idMission);
            if (!b && mission.Done)
            {
                PC.WinPoints(mission.Points);
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
        if (!NetworkManager.Singleton.IsServer)
        {
            Destroy(gameObject);
            return;
        }
        List<GameEvent<T1, T2, T3>> listEvents = new List<GameEvent<T1, T2, T3>>();
        missions = Resources.LoadAll<Mission<T1, T2, T3>>("Missions/lvl" + lvl + "/MissionAsset/").ToList();

        foreach (Mission<T1, T2, T3> mission in missions)
        {
            mission.initValues(this.gameObject).Response.AddListener(this.RaisedEvent);
        }
        foreach (Mission<T1, T2, T3> mission in missions)
        {
            missionsInitialValues.Add(mission.idMission, mission.Done);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerControlls PC = other.gameObject.GetComponent<PlayerControlls>();
        if (PC == null) return;

        foreach (Mission<T1, T2, T3> mission in missions)
        {
            bool b = missionsInitialValues.GetValueOrDefault(mission.idMission);
            if (!b && mission.Done)
            {
                PC.WinPoints(mission.Points);
            }
        }
    }
    public void RaisedEvent(GameEvent<T1, T2, T3> gameEvent, T1 parameter1, T2 parameter2, T3 parameter3)
    {
        foreach (Mission<T1, T2, T3> mission in missions)
            mission.execute(gameEvent, parameter1, parameter2, parameter3);
    }
}
