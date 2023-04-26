using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (fileName = "SpawnerEnemiesInformation", menuName = "Enemy/SpawnerEnemiesInformation")]
public class SpawnerEnemies : ScriptableObject
{
    public List<SpawnInfo> listPositionSpawn;
    public List<Vector3> randomPositions;
}

[Serializable]
public struct SpawnInfo
{
    public Vector3 spawnPosition;   
    public EnemyType enemyType; 
}
public enum EnemyType
{
    PYROGNATHIAN, QUIRAXIAN, THRAAXIAN, ZORGONIAN
}