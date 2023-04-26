using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class SpawnerEnemiesBehaviour : NetworkBehaviour
{
    public SpawnerEnemies Info;
    public GameObject Quiraxian;
    public GameObject Thraaxian;
    public GameObject Zorgonian;
    public GameObject Pyrognathian;
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
        Spawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        Spawn();
        this.enabled = false;
    }
    private void Spawn()
    {
        foreach (SpawnInfo info in Info.listPositionSpawn)
        {
            switch (info.enemyType)
            {
                case EnemyType.THRAAXIAN:
                    GameObject t = Instantiate(Thraaxian);
                    t.GetComponent<EnemyBehaviour>().randomPositions = Info.randomPositions;
                    Debug.Log("AQUI SE LO PONGO");
                    t.transform.position = info.spawnPosition;
                    t.GetComponent<NetworkObject>().Spawn();
                    break;
                case EnemyType.QUIRAXIAN:
                    GameObject q = Instantiate(Quiraxian);
                    q.GetComponent<EnemyBehaviour>().randomPositions = Info.randomPositions;
                    Debug.Log("AQUI SE LO PONGO");
                    q.transform.position = info.spawnPosition;
                    q.GetComponent<NetworkObject>().Spawn();
                    break;
                case EnemyType.ZORGONIAN:
                    GameObject z = Instantiate(Zorgonian);
                    z.GetComponent<EnemyBehaviour>().randomPositions = Info.randomPositions;
                    Debug.Log("AQUI SE LO PONGO");
                    z.transform.position = info.spawnPosition;
                    z.GetComponent<NetworkObject>().Spawn();
                    break;
                case EnemyType.PYROGNATHIAN:
                    GameObject p = Instantiate(Pyrognathian);
                    p.GetComponent<EnemyBehaviour>().randomPositions = Info.randomPositions;
                    Debug.Log("AQUI SE LO PONGO");
                    p.transform.position = info.spawnPosition;
                    p.GetComponent<NetworkObject>().Spawn();
                    break;
                default: break;
            }
        }
    }
}
