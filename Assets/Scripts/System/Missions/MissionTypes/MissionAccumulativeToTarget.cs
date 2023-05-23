using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
[CreateAssetMenu(fileName = "missionAccumulativeToTarget", menuName = "Mission/OneParam/missionAccumulativeToTarget")]
public class MissionAccumulativeToTarget : Mission<int>
{
    [SerializeField]
    private int totalAmount;
    [SerializeField]
    private int currentAmount;
    public override void execute(GameEvent<int> gameEvent, int parameter)
    {
        if(gameEvent.GetType() == typeof(MissionOneParamIntEvent)) {
            currentAmount += parameter;
            Done = totalAmount > currentAmount;
        }
        if (Done)
        {
            changeStatus?.Raise(idMission, Description, Color.green);
        }
        else
        {
            changeStatus?.Raise(idMission, Description + " Progression(" + currentAmount + " / " + totalAmount + ")", new Color(0, 255, 255));
        }
    }

    public override GameEventListener<int> initValues(GameObject gameObject)
    {
        changeStatus = Resources.Load<EventChangeTextMissions>("Events/EventChangeTextMissions");
        currentAmount = 0;
        MissionOneParamIntListener listener = gameObject.AddComponent<MissionOneParamIntListener>();
        listener.Event = this.Event;
        listener.Response = new UnityEngine.Events.UnityEvent<GameEvent<int>, int>();
        return listener;
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
