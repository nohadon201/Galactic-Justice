using UnityEngine.Events;
using UnityEngine;
public class MissionEventListener : MonoBehaviour, IEventListener
{
    [Tooltip("Event to register with.")]
    public GameEvent Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent<GameEvent> Response;

    private void Start()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised()
    {
        Response.Invoke(Event);
    }
}