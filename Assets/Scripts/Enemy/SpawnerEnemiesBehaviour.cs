using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using JetBrains.Annotations;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class SpawnerEnemiesBehaviour : NetworkBehaviour
{
    public GameObject Quiraxian;
    public GameObject Thraaxian;
    public GameObject Zorgonian;
    public GameObject Pyrognathian;
    public TypeSpawn type;
    [SerializeField]
    public List<SpawnerEnemies> InfoList = new List<SpawnerEnemies>();
    [SerializeField]
    public List<SpawnerTrigger> InfoTrigger = new List<SpawnerTrigger>();

    private void Awake()
    {

        List<GameObject> a = (Resources.LoadAll<GameObject>("Prefabs/Enemy/")).ToList();
        foreach (GameObject enemy in a)
        {
            EnemyBehaviour enemyComponent = enemy.GetComponent<EnemyBehaviour>();

            if (enemyComponent is Quiraxian)
            {
                Quiraxian = enemy;
            }
            else if (enemyComponent is Thraaxian)
            {
                Thraaxian = enemy;
            }
            else if (enemyComponent is Pyrognathian)
            {
                Pyrognathian = enemy;
            }
            else
            {
                Zorgonian = enemy;
            }
        }
    }
    void Start()
    {
        if (!IsServer) { this.enabled = false; return; }
        GameObject go = Resources.Load<GameObject>("Prefabs/Enemy/GeneralPool");
        Instantiate(go).GetComponent<NetworkObject>().Spawn();
        if(type == TypeSpawn.ON_START)
            Spawn();
    }
    private void Spawn()
    {

        foreach (SpawnerEnemies Info in InfoList)
        {
            foreach (SpawnInfo info in Info.listPositionSpawn)
            {
                switch (info.enemyType)
                {
                    case EnemyType.THRAAXIAN:
                        GameObject t = Instantiate(Thraaxian);
                        t.GetComponent<EnemyBehaviour>().randomPositions = Info.randomPositions;
                        t.transform.position = info.spawnPosition;
                        t.GetComponent<NetworkObject>().Spawn();
                        break;
                    case EnemyType.QUIRAXIAN:
                        GameObject q = Instantiate(Quiraxian);
                        q.GetComponent<EnemyBehaviour>().randomPositions = Info.randomPositions;
                        q.transform.position = info.spawnPosition;
                        q.GetComponent<NetworkObject>().Spawn();
                        break;
                    case EnemyType.ZORGONIAN:
                        GameObject z = Instantiate(Zorgonian);
                        z.GetComponent<EnemyBehaviour>().randomPositions = Info.randomPositions;
                        z.transform.position = info.spawnPosition;
                        z.GetComponent<NetworkObject>().Spawn();
                        break;
                    case EnemyType.PYROGNATHIAN:
                        GameObject p = Instantiate(Pyrognathian);
                        p.GetComponent<EnemyBehaviour>().randomPositions = Info.randomPositions;
                        p.transform.position = info.spawnPosition;
                        p.GetComponent<NetworkObject>().Spawn();
                        break;
                    default: break;
                }
            }
        }
    }
    public void SpawnByTutorialer(StateScene state)
    {
        if (type != TypeSpawn.ON_EVENT) return;
        SpawnStateServerRpc((int)state);
    }
    public void SpawnByTrigger(StateScene state)
    {
        if(type!= TypeSpawn.ON_TRIGGER) return;
        SpawnStateServerRpc((int)state);  
    }
    [ServerRpc]
    public void SpawnStateServerRpc(int state)
    {
        Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA SERVIDOR");
        foreach(SpawnerTrigger spwnTrigger in InfoTrigger)
        {
            if(spwnTrigger.id == state)
            {
                foreach (SpawnInfo info in spwnTrigger.spawner.listPositionSpawn)
                {
                    switch (info.enemyType)
                    {
                        case EnemyType.THRAAXIAN:
                            GameObject t = Instantiate(Thraaxian);
                            t.GetComponent<EnemyBehaviour>().randomPositions = spwnTrigger.spawner.randomPositions;
                            t.transform.position = info.spawnPosition;
                            t.GetComponent<NetworkObject>().Spawn();
                            break;
                        case EnemyType.QUIRAXIAN:
                            GameObject q = Instantiate(Quiraxian);
                            q.GetComponent<EnemyBehaviour>().randomPositions = spwnTrigger.spawner.randomPositions;
                            q.transform.position = info.spawnPosition;
                            q.GetComponent<NetworkObject>().Spawn();
                            break;
                        case EnemyType.ZORGONIAN:
                            GameObject z = Instantiate(Zorgonian);
                            z.GetComponent<EnemyBehaviour>().randomPositions = spwnTrigger.spawner.randomPositions;
                            z.transform.position = info.spawnPosition;
                            z.GetComponent<NetworkObject>().Spawn();
                            break;
                        case EnemyType.PYROGNATHIAN:
                            GameObject p = Instantiate(Pyrognathian);
                            p.GetComponent<EnemyBehaviour>().randomPositions = spwnTrigger.spawner.randomPositions;
                            p.transform.position = info.spawnPosition;
                            p.GetComponent<NetworkObject>().Spawn();
                            break;
                        default: break;
                    }
                }
            }
        }
    }
}
public enum TypeSpawn
{
    ON_START, ON_TRIGGER, ON_EVENT
}