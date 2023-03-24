using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class aaaScript : MonoBehaviour
{
    public Transform t;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            RaycastHit hit;
            Vector3 v = t.position - transform.position;    
            if(Physics.Raycast(transform.position, v.normalized, out hit, 50000, 1))
            {
                Debug.DrawLine(transform.position, hit.point);
                if(hit.transform.name == "Thraaxian")
                {
                    Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                    hit.transform.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * 10000, hit.point);
                }
            }
        }
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
