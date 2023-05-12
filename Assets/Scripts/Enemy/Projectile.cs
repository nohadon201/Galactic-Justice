using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private Vector3 enemy;
    private float range;
    private float damage;
    public void ShootBullet(Vector3 enemy, Vector3 directionOfPlayer, float velocity, float damage, float range)
    {
        this.range = range; 
        this.enemy = enemy;
        transform.position = enemy;
        GetComponent<Rigidbody>().velocity = directionOfPlayer * velocity ;
        this.damage = damage;
        StartCoroutine(disaplayNone());
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(!IsServer) {
            Destroy(GetComponent<Rigidbody>());
        }  
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        
        if(other.transform.tag == "Player") {
            PlayerControlls pc = other.transform.GetComponentInParent<PlayerControlls>();
            
            if (pc != null) 
            {
                pc.GetDamageClientRpc(this.damage, pc.IsOwner); 
            }
        }

        GeneralPool.Instance.returnProjectile(gameObject);
    }
    
    private IEnumerator disaplayNone()
    {
        bool a = false; 
        while(!a)
        {
            Vector3 v = enemy - transform.position;
            a = Mathf.Abs(v.x) > range || Mathf.Abs(v.y) > range || Mathf.Abs(v.z) > range;
            yield return new WaitForSeconds(1f);
            
        }

        GeneralPool.Instance.returnProjectile(gameObject);
    }
}
