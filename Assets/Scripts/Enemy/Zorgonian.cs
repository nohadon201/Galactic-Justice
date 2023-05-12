using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Zorgonian : EnemyBehaviour
{
    private bool iker;
    public float forceApliedToPlayer;
    private bool iker2;
    private float ycomponent;
    private void Start()
    {
        if (!IsServer) return;
        // Zorgonian custom values
        forceApliedToPlayer = 9;
        ycomponent = Mathf.Sin(45 * Mathf.PI / 180);
        //  Zorgonian unique values
        filter.agentTypeID = 0;
        filter.areaMask = 7;
        velocity = 10.0f;
        currentPath = new();
        damagePerImpact = 100f;
        maxHealth = 200;
        currentHealth = maxHealth;
        RangeAttack = 2f;
        CooldownAttack = 1f;
        //  Generic Enemy default values+
        SetLayers();
        navmeshIndexPosition = 0;
        rb = GetComponent<Rigidbody>();
        PlayerForgive = false;
        iker2 = false;
        indexCurrentPointAlert = UnityEngine.Random.Range(0, randomPositions.Count);
        CalculatePath(randomPositions[indexCurrentPointAlert]);
        ChangeState(StateOfEnemy.PATROL);
    }
    /*
     * ################################### Range Attack ##############################################
     */
    private bool CheckIfItsClosePlayer()
    {
        Vector3 v = (playerRef.transform.position - transform.position);
        return (Mathf.Abs(v.x) <= RangeAttack) && (Mathf.Abs(v.z) <= RangeAttack);

    }
    private bool CheckIfItsFarAwayPlayer()
    {
        Vector3 v = (playerRef.transform.position - transform.position);
        return (Mathf.Abs(v.x) > RangeAttack) || (Mathf.Abs(v.z) > RangeAttack);
    }

    /*
     * ################################### Rotation and Movement with Navmesh ##############################################
     */
    private void RotationWithTheTarget(Vector3 target)
    {
        float x = target.x - transform.position.x;
        float z = target.z - transform.position.z;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(new Vector3(x, 0, z)), 1.5f);
    }
    private void CalculatePath(Vector3 position)
    {
        currentPath.ClearCorners();
        NavMesh.CalculatePath(transform.position, position, filter, currentPath);
        navmeshIndexPosition = 0;
    }

    /*
     * ################################### Attack ##############################################
     */
    private IEnumerator AttackingCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(CooldownAttack);
            Attack();
        }
    }
    private void Attack()
    {
        
        Vector3 direction = playerRef.transform.position - transform.position;
        direction.Normalize();
        RaycastHit hit;
        if(Physics.Raycast(transform.position + (transform.forward*0.5f), direction, out hit, 10f, obstructionMask))
        {
            if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Target"))
            {
                Vector3 dir = playerRef.position-transform.position;
                PlayerControlls pc = hit.transform.gameObject.GetComponent<PlayerControlls>();  
                dir.y = ycomponent;
                pc.GetPushedClientRpc(dir * forceApliedToPlayer, pc.IsOwner);    
                pc.GetDamageClientRpc(damagePerImpact, pc.IsOwner);
            }
        }
    }
    /*
    * ################################### DeathCase ##############################################
    */
    public override void OnEnemyFall()
    {
        base.OnEnemyFall();
        if (transform.position.y < -20)
        {
            OnEnemyDeathWich?.Raise((int)EnemyType.ZORGONIAN);
        }
    }
    public override void GetHit(float damage)
    {
        base.GetHit(damage);
        if (currentHealth <= 0)
        {
            OnEnemyDeathWich.Raise((int)EnemyType.ZORGONIAN);
        }
    }
    /*
     * ################################### State Machine ##############################################
     */
    //          PATROL
    protected override void InitStatePatrol()
    {
        if (findPlayer != null)
        {
            StopCoroutine(findPlayer);
        }
        CalculatePath(randomPositions[indexCurrentPointAlert]);
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
        findPlayer = StartCoroutine(FindPlayer());
    }

    protected override void UpdateStateFollowing()
    {
        RotationWithTheTarget(playerRef.position);
    }

    protected override void FixedUpdateStateFollowing()
    {
        setTheVelocity();
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
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        attack = StartCoroutine(AttackingCoroutine());
        if (forgivePlayer != null)
        {
            StopCoroutine(forgivePlayer);
        }
        iker2 = false;

    }

    protected override void UpdateStateAttack()
    {
        if (CheckIfItsFarAwayPlayer() && !iker2)
        {
            StartCoroutine(StartFollowing());
        }
        RotationWithTheTarget(playerRef.position);
    }

    protected override void FixedUpdateStateAttack()
    {

    }

    protected override void ExitStateAttack()
    {
        StopCoroutine(attack);
    }
    //      COROUTINES
    protected override IEnumerator FindPlayer()
    {
        while (true)
        {
            if (currentState != StateOfEnemy.PATROL) CalculatePath(playerRef.position);
            yield return new WaitForSeconds(1);
        }
    }
    private IEnumerator StartFollowing()
    {
        iker2 = true;
        StopCoroutine(attack);
        yield return new WaitForSeconds(2f);
        ChangeState(StateOfEnemy.FOLLOWING);

    }
    protected override IEnumerator ForgivePlayer()
    {
        PlayerForgive = true;
        iker = true;
        yield return new WaitForSeconds(0.2f);
        iker = false;
        yield return new WaitForSeconds(3);
        ChangeState(StateOfEnemy.PATROL);
        PlayerForgive = false;
    }
    //      OnPlayerVision Functions
    protected override void OnPlayerSeen()
    {
        Debug.Log("OnPlayerSeen");
        switch (currentState)
        {
            case StateOfEnemy.PATROL:
                ChangeState(StateOfEnemy.FOLLOWING);
                break;
            case StateOfEnemy.FOLLOWING:
                if (forgivePlayer != null)
                {
                    if (!iker)
                    {
                        StopCoroutine(forgivePlayer);
                        if (PlayerForgive) PlayerForgive = false;
                    }
                }
                break;
            case StateOfEnemy.ATTACK:
            default:
                break;
        }
    }

    protected override void OnPlayerAway()
    {
        Debug.Log("OnPlayerAway");
        switch (currentState)
        {
            case StateOfEnemy.PATROL:
                break;
            case StateOfEnemy.ATTACK:
                if (!PlayerForgive)
                {
                    forgivePlayer = StartCoroutine(ForgivePlayer());
                }
                if (!iker2)
                {
                    StartCoroutine(StartFollowing());
                }
                break;
            case StateOfEnemy.FOLLOWING:
                if (!PlayerForgive)
                {
                    forgivePlayer = StartCoroutine(ForgivePlayer());
                }
                break;
            default:
                break;
        }
    }
    //      OTHER STATES
    protected override void InitStateCrazy()
    {

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
        if (findPlayer != null)
        {
            StopCoroutine(findPlayer);
        }
        RunAwayPlayer = StartCoroutine(RunAwayFromPlayer());
    }

    protected override void UpdateStateTerrified()
    {

    }

    protected override void FixedUpdateStateTerrified()
    {
        setTheVelocity();
    }

    protected override void ExitStateTerrified()
    {
        StopCoroutine(RunAwayPlayer);
    }
}
