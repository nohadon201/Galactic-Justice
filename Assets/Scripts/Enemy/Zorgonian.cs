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
        filter.agentTypeID = 0;
        filter.areaMask = 7;
        velocity = 4f;
        currentPath = new();
        damagePerImpact = 11.5f;
        maxHealth = 30;
        currentHealth = maxHealth;
        RangeAttack = 3;
        CooldownAttack = 3f;
        //  Generic Enemy default values
        SetRandomPostions();
        navmeshIndexPosition = 0;
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        indexCurrentPointAlert = Random.Range(0, randomPositions.Count);
        CalculatePath(randomPositions[indexCurrentPointAlert]);
        ChangeState(StateOfEnemy.PATROL);
    }
    /*
     * ################################### Range Attack ##############################################
     */
    private bool CheckIfItsClosePlayer()
    {
        Vector3 v = (playerRef.transform.position - transform.position);
        return (v.x < RangeAttack && v.x > -RangeAttack) && (v.z < RangeAttack && v.z > -RangeAttack);

    }
    private bool CheckIfItsFarAwayPlayer()
    {
        Vector3 v = (playerRef.position - transform.position);
        Debug.Log((Mathf.Abs(v.x) > RangeAttack || Mathf.Abs(v.z) > RangeAttack));
        return (Mathf.Abs(v.x) > RangeAttack || Mathf.Abs(v.z) > RangeAttack);
    }

    /*
     * ################################### Rotation and Movement with Navmesh ##############################################
     */
    private void RotationWithTheTarget(Vector3 target)
    {
        Vector3 forward = target - transform.position;
        float x = target.x - transform.position.x;
        float z = target.z - transform.position.z;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(new Vector3(x, transform.position.y, z)), 1.5f);
    }
    private void CalculatePath(Vector3 position)
    {
        currentPath.ClearCorners();
        if (!NavMesh.CalculatePath(transform.position, position, filter, currentPath))
        {
            Debug.Log("No Path finded");
        }
        navmeshIndexPosition = 0;
    }


    /*
     * ################################### Attack ##############################################
     */
    private IEnumerator AttackingCoroutine()
    {
        while (true)
        {
            Attack();
            yield return new WaitForSeconds(CooldownAttack);
        }

    }
    public void Attack()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (transform.forward + new Vector3(0, 0.1f, 0)), out hit, 7, obstructionMask))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red, 2f);  
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Target"))
            {
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * 1000, hit.point);
            }
        }
    }
    /*
     * ################################### State Machine ##############################################
     */
    //          PATROL
    protected override void OnPlayerSeen()
    {
        ChangeState(StateOfEnemy.FOLLOWING);
    }

    protected override void InitStatePatrol()
    {
        Debug.Log("Patrol");
    }

    protected override void UpdateStatePatrol()
    {
        RotationWithTheTarget(randomPositions[indexCurrentPointAlert]);
    }

    protected override void FixedUpdateStatePatrol()
    {
        if (KeepGoingPatrolPoint())
        {
            CalculatePath(randomPositions[indexCurrentPointAlert]);
        }
        setTheVelocity();
    }

    protected override void ExitStatePatrol()
    {

    }

    //          FOLLOWING

    protected override void InitStateFollowing()
    {
        Debug.Log("Following");
        findPlayer = StartCoroutine(FindPlayer());
    }

    protected override void UpdateStateFollowing()
    {
        RotationWithTheTarget(playerRef.position);
    }

    protected override void FixedUpdateStateFollowing()
    {
        Debug.Log("Following");
        setTheVelocity();
        Debug.Log(rb.velocity);
        if (CheckIfItsClosePlayer())
        {
            ChangeState(StateOfEnemy.ATTACK);
        }
    }

    protected override void ExitStateFollowing()
    {
        StopCoroutine(findPlayer);
    }

    //          ATTACK

    protected override void InitStateAttack()
    {
        Debug.Log("Attack");
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        attack = StartCoroutine(AttackingCoroutine());
    }

    protected override void UpdateStateAttack()
    {
        if (CheckIfItsFarAwayPlayer())
        {
            ChangeState(StateOfEnemy.FOLLOWING);
        }
    }

    protected override void FixedUpdateStateAttack()
    {

    }

    protected override void ExitStateAttack()
    {
        StopCoroutine(attack);
    }

    protected override IEnumerator FindPlayer()
    {
        while (true)
        {
            CalculatePath(playerRef.position);
            yield return new WaitForSeconds(1);
        }
    }
    protected override IEnumerator ForgivePlayer()
    {
        yield return new WaitForSeconds(7);
        ChangeState(StateOfEnemy.PATROL);
    }
    protected override void OnPlayerAway()
    {
        forgivePlayer = StartCoroutine(ForgivePlayer());
    }

    protected override void InitStateCrazy()
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdateStateCrazy()
    {
        throw new System.NotImplementedException();
    }

    protected override void FixedUpdateStateCrazy()
    {
        throw new System.NotImplementedException();
    }

    protected override void ExitStateCrazy()
    {
        throw new System.NotImplementedException();
    }

    protected override void InitStateTerrified()
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdateStateTerrified()
    {
        throw new System.NotImplementedException();
    }

    protected override void FixedUpdateStateTerrified()
    {
        throw new System.NotImplementedException();
    }

    protected override void ExitStateTerrified()
    {
        throw new System.NotImplementedException();
    }
}
