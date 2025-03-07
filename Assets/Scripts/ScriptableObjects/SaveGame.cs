using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SaveGameSO", menuName = "System/SaveGame")]
public class SaveGame : ScriptableObject
{
    [Header("Host Variables")]
    public int LevelsCompleted;
    public PlayerInfo playerHost;
    public List<PowerBulletSO> powerBulletHost;
    [Header("")]
    [Header("Client Variables")]
    public PlayerInfo playerClient;
    public List<PowerBulletSO> powerBulletClient;
    [Header("Missions Completed")]
    public List<Mission> missions;
    public List<Mission<int>> missionIntParam;
}
