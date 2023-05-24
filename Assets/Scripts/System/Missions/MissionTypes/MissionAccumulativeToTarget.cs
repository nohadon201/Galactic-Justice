using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
[CreateAssetMenu(fileName = "missionAccumulativeToTarget", menuName = "Mission/OneParam/missionAccumulativeToTarget")]
public class MissionAccumulativeToTarget : Mission<float>
{
    [SerializeField]
    private float totalAmount;
    [SerializeField]
    private float currentAmount;
    public override void execute(GameEvent<float> gameEvent, float parameter)
    {
        Debug.Log("Uno");
        if (gameEvent == this.Event)
        {
            currentAmount += parameter;
            Done = totalAmount <= currentAmount;
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

    public override GameEventListener<float> initValues(GameObject gameObject)
    {
        changeStatus = Resources.Load<EventChangeTextMissions>("Events/EventChangeTextMissions");
        currentAmount = 0;
        MissionOneParamFloatListener listener = gameObject.AddComponent<MissionOneParamFloatListener>();
        listener.Event = this.Event;
        listener.Response = new UnityEngine.Events.UnityEvent<GameEvent<float>, float>();
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
