using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WinPointsListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public EventPoints Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent<int> Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(int parameter)
    {
        Debug.Log("Al menos llegas hasta aquí???");
        Response.Invoke(parameter);
    }

}
