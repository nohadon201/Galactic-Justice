using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "Multiplayer", menuName = "Multiplayer Information/Connection Information")]
public class MultiplayerInfo : ScriptableObject
{
    public bool Host;
    public int NumberOfPlayers;
    public bool connected;
}
