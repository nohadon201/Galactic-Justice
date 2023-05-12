using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="SpawnerTrigger", menuName = "Enemy/SpawnerTriggerInformation")]
public class SpawnerTrigger : ScriptableObject
{
    public int id;
    public SpawnerEnemies spawner;
}
