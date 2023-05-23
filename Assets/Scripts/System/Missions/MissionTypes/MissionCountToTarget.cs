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
        changeStatus = Resources.Load<EventChangeTextMissions>("Events/EventChangeTextMissions");
        if (Done)
        {
            changeStatus?.Raise(idMission, Description, Color.green);
        }
        else
        {
            changeStatus?.Raise(idMission, Description + " Progression(" + currentAmount + " / " + totalAmount + ")", new Color(0, 255, 255));
        }
        if (!Done)
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
        if(Done)
        {
            changeStatus?.Raise(idMission, Description, Color.green);
        }
        else
        {
            changeStatus?.Raise(idMission, Description+" Progression("+currentAmount+" / "+totalAmount+")", new Color(0, 255, 255));
        }
    }

    public override void updateText()
    {
        if (Done)
        {
            changeStatus?.Raise(idMission, Description, Color.green);
        }
        else
        {
            changeStatus?.Raise(idMission, Description + " Progression(" + currentAmount + " / " + totalAmount + ")", new Color(0, 255, 255));
        }
    }
}
