using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 enemy;
    private float range;
    private float damage;
    public void ShootBullet(Vector3 enemy, Vector3 directionOfPlayer, float velocity, float damage, float range){
        this.range = range; 
        this.enemy = enemy;
        transform.position = enemy;
        GetComponent<Rigidbody>().velocity = directionOfPlayer * velocity ;
        this.damage = damage;
        StartCoroutine(disaplayNone());
    }
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("aaaa");
        if(other.transform.tag == "Player") {
            //other.gameObject.GetComponent<PlayerControlls>().Damage(this.damage);
            this.gameObject.SetActive(false);
            //Debug.Log("eeeee");
        }
        else
        {
            this.gameObject.SetActive(false);
        }
        
    }
    private IEnumerator disaplayNone()
    {
        bool a = false; 
        while(!a)
        {
            Vector3 v = enemy - transform.position;
            a = Mathf.Abs(v.x) > range || Mathf.Abs(v.y) > range || Mathf.Abs(v.z) > range;
            //Debug.Log("aaaa");
            yield return new WaitForSeconds(1f);
            
        }
        this.gameObject.SetActive(false);
    }
}
