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
    public List<SlotOfMemory> slotsHost;
    public PlayerInfo playerClient;
    public List<SlotOfMemory> slotsClient;
    public List<PowerBulletSO> powerBulletClient;
    [Header("Missions Completed")]
    public List<Mission> missions;

}
