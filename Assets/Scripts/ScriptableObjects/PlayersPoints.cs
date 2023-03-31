using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayersPoints", menuName = "Player/PlayerPoints")]
public class PlayersPoints : ScriptableObject
{
    public int Points;
    public void DebugPoints()
    {
        Debug.Log("You Win: "+Points+" points");
    }
}
