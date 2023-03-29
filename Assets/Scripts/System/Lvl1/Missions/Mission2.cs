using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
[CreateAssetMenu]
public class Mission2 : Mission<int>
{
    Mission2Event m_event;
    private int damageToSuperate;
    private int damageAccumulated;
    public override void execute(GameEvent<int> gameEvent, int parameter)
    {
        if(gameEvent.GetType() == typeof(Mission2Event)) {
            //damageAccumulated += gameEvent.
            Done = damageAccumulated > damageToSuperate;
        }
    }

    public override void initValues()
    {
        damageAccumulated = 0;
        Title = "The hitman";
        Description = "Make X damage or more";
        Points = 30;
        damageToSuperate = 500;
    }
}
