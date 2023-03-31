using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "missionCountToTarget", menuName = "Mission/Non Params/missionCountToTarget")]
public class MissionCountToTarget : Mission
{
    
    public int totalAmount;
    public int currentAmount;

    public override void initValues()
    {
        if(!Done)
        {
            currentAmount = 0;
        }
    }
    public override void execute(GameEvent gameEvent)
    {
        if(!Done && Event == gameEvent) {
            currentAmount++;
            Done = currentAmount >= totalAmount;
        }
    }     
}
