using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerTutorialer : MonoBehaviour
{
    private bool spawned;
    [SerializeField]
    private StateScene m_Estado;
    [SerializeField]
    private UnityEvent<StateScene, GameObject> m_Accion;
    [SerializeField] private UnityEvent<StateScene> spawnEvent;
    public bool Closed;
    BoxCollider bc;
    private void Awake()
    {
        spawned= false; 
        Closed= false;
        bc = GetComponent<BoxCollider>();   
    }
    private void OnTriggerEnter(Collider other)
    {
        if(Closed) return;
        if (other.transform.tag != "Player") return;
        m_Accion.Invoke(m_Estado, other.gameObject);
        if (!spawned)
        {
            spawnEvent.Invoke(m_Estado);
            spawned= true;  
        }
    }
    public void MakeEncerrona(bool entrar)
    {
        Closed = entrar;
        if (Closed)
            bc.isTrigger = false;
        else
            bc.isTrigger = true;
    }
}
