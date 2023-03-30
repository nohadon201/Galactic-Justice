using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class aaaScript : MonoBehaviour
{
    [SerializeField]
    private MissionOneParamIntEvent mission1Event;
    public Transform t;
    // Start is called before the first frame update
    void Start()
    {
        mission1Event = Resources.Load<MissionOneParamIntEvent>("Events/OnTestIntParam");   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void cosa2() {
        mission1Event?.Raise(250);
    }
    IEnumerator cosa(RaycastHit hit)
    {
        hit.transform.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        hit.transform.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        
        yield return new WaitForSeconds(1);
        hit.transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        hit.transform.gameObject.GetComponent<NavMeshAgent>().enabled = true;
    }
}
