using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerTutorialer : MonoBehaviour
{
    [SerializeField]
    private StateScene m_Estado;
    [SerializeField]
    private UnityEvent<StateScene, GameObject> m_Accion;
    public bool Closed;
    BoxCollider bc;
    private void Awake()
    {
        Closed= false;
        bc = GetComponent<BoxCollider>();   
    }
    private void OnTriggerEnter(Collider other)
    {
        if(Closed) return;
        if (other.transform.tag != "Player") return;
        m_Accion.Invoke(m_Estado, other.gameObject);
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
