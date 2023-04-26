using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private Vector3 enemy;
    private float range;
    private float damage;
    private MeshRenderer meshRenderer;
    private Light Light;
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
                pc.Damage(this.damage); 
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
    /*
    [ClientRpc]
    public void SetTrueRendClientRpc(Color c)
    {
        if (IsServer) return;
        Debug.Log("ENTRA HOST TAMBIEN");
        meshRenderer.material.color = c;
        Light.color = c;
        meshRenderer.enabled = true;
        Light.enabled = true;
    }
    [ClientRpc]
    public void SetFalseRendClientRpc()
    {
        if (IsServer) return;
        meshRenderer.enabled = false;
        Light.enabled = false;
    }
    */
}
