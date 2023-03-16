using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float damage;
    public void ShootBullet(Vector3 enemy, Vector3 directionOfPlayer, float velocity, float damage){
        transform.position = enemy;
        GetComponent<Rigidbody>().velocity = directionOfPlayer * velocity ;
        this.damage = damage;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player") {
            //other.gameObject.GetComponent<PlayerControlls>().Damage(this.damage);
            this.gameObject.SetActive(false);
        }
        
    }
    void Update()
    {
        
    }
}
