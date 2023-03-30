using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
[CreateAssetMenu(fileName = "missionAccumulativeToTarget", menuName = "Mission/OneParam/missionAccumulativeToTarget")]
public class MissionAccumulativeToTarget : Mission<int>
{
    [SerializeField]
    private int damageToSuperate;
    [SerializeField]
    private int damageAccumulated;
    public override void execute(GameEvent<int> gameEvent, int parameter)
    {
        if(gameEvent.GetType() == typeof(MissionOneParamIntEvent)) {
            damageAccumulated += parameter;
            Done = damageAccumulated > damageToSuperate;
        }
    }

    public override GameEventListener<int> initValues(GameObject gameObject)
    {
        damageAccumulated = 0;
        MissionOneParamIntListener listener = gameObject.AddComponent<MissionOneParamIntListener>();
        listener.Event = this.Event;
        listener.Response = new UnityEngine.Events.UnityEvent<GameEvent<int>, int>();
        return listener;
    }
}
