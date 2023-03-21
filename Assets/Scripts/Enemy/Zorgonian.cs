using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Zorgonian : EnemyBehaviour
{
    
    void Awake()
    {
        //  Pyrognathian unique values
        GetComponent<NavMeshAgent>().speed= 10.0f;
        damagePerImpact = 20f;
        maxHealth = 20;
        currentHealth = maxHealth;
        RangeAttack = 10;
        //  Generic Enemy default values
        SetRandomPostions();
        currentState = StateOfEnemy.PATROL;
    }
    private void Start()
    {
        indexCurrentPointAlert = Random.Range(0, randomPositions.Count);
    }

    void Update()
    {
        if(currentState == StateOfEnemy.PATROL) {
            KeepGoingAlertPoint();
            GetComponent<NavMeshAgent>().destination = randomPositions[indexCurrentPointAlert];
        }else if(currentState == StateOfEnemy.FOLLOWING)
        {
            if (CheckIfItsClose())
            {
                currentState = StateOfEnemy.ATTACK;
                StartCoroutine(AttackingCoroutine());
                GetComponent<NavMeshAgent>().destination = transform.position;
            }
            else
            {
                GetComponent<NavMeshAgent>().destination = playerRef.transform.position;
            }
                
        }else if(currentState == StateOfEnemy.ATTACK)
        {
            if(CheckIfItsFarAway()) { 
                currentState = StateOfEnemy.FOLLOWING;
            }
        }
    }

    private IEnumerator AttackingCoroutine()
    {
        while (true)
        {
            Shoot(playerRef.transform.position);
            yield return new WaitForSeconds(1);
        }
    }
    public void Shoot(Vector3 v)
    {
        GameObject pool = GeneralPool.Instance.poolThraaxian;
        for(int a = 0; a < pool.transform.childCount; a++)
        {
            if(!pool.transform.GetChild(a).gameObject.activeSelf)
            {
                Vector3 diff = (v - transform.position).normalized;
                Vector3 position = transform.position + (transform.forward * 3);
                pool.transform.GetChild(a).gameObject.SetActive(true);
                pool.transform.GetChild(a).gameObject.GetComponent<Projectile>().ShootBullet(position, diff, 7,damagePerImpact);
                break;
            }
        }
    }
    public void KeepGoingAlertPoint()
    {
        Vector3 v = (randomPositions[indexCurrentPointAlert] - transform.position);
        if (v.x < 1 && v.z < 1)
        {
            int a = indexCurrentPointAlert;
            while (a == indexCurrentPointAlert) { 
                a = Random.Range(0, randomPositions.Count);
            }
            indexCurrentPointAlert = a;
        }
    }
    public bool CheckIfItsClose()
    {
        Vector3 v = (playerRef.transform.position - transform.position);
        return (v.x < RangeAttack && v.x > -RangeAttack) && (v.z < RangeAttack && v.z > -RangeAttack);

    }
    public bool CheckIfItsFarAway()
    {
        Vector3 v = (playerRef.transform.position - transform.position);
        return (v.x > RangeAttack || v.x < -RangeAttack) || (v.z > RangeAttack || v.z < -RangeAttack);
    }
    private void FixedUpdate()
    {
        
    }
    private void LateUpdate()
    {
        
    }

    protected override void OnPlayerSeen()
    {
        ChangeState(StateOfEnemy.FOLLOWING);
    }

    protected override void InitStatePatrol()
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdateStatePatrol()
    {
        throw new System.NotImplementedException();
    }

    protected override void FixedUpdateStatePatrol()
    {
        throw new System.NotImplementedException();
    }

    protected override void ExitStatePatrol()
    {
        throw new System.NotImplementedException();
    }

    protected override void InitStateFollowing()
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdateStateFollowing()
    {
        throw new System.NotImplementedException();
    }

    protected override void FixedUpdateStateFollowing()
    {
        throw new System.NotImplementedException();
    }

    protected override void ExitStateFollowing()
    {
        throw new System.NotImplementedException();
    }

    protected override void InitStateAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdateStateAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void FixedUpdateStateAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void ExitStateAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator FindPlayer()
    {
        throw new System.NotImplementedException();
    }
}
