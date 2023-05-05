using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class Quiraxian : EnemyBehaviour
{
    private bool iker;
    [SerializeField] private Material materialProjectile;
    [SerializeField] private Color ColorProjectile;
    private void Start()
    {
        if (!IsServer) return;
        //  Quiraxian unique values
        filter.agentTypeID = 0;
        filter.areaMask = 7;
        velocity = 10.0f;
        currentPath = new();
        damagePerImpact = 7f;
        maxHealth = 95;
        currentHealth = maxHealth;
        RangeAttack = 5;
        CooldownAttack = 1f;
        //  Generic Enemy default values+
        SetLayers();
        navmeshIndexPosition = 0;
        rb = GetComponent<Rigidbody>();
        PlayerForgive = false;
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
        Vector3 forward = target - transform.position;
        float x = target.x - transform.position.x;
        float z = target.z - transform.position.z;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(new Vector3(x, transform.position.y, z)), 1.5f);
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
            Shoot(playerRef.transform.position, GeneralPool.Instance.getProjectile());
            yield return new WaitForSeconds(CooldownAttack);
        }
    }
    public void Shoot(Vector3 v, GameObject projectile)
    {
        Vector3 position = transform.position + (transform.forward * 1.5f);
        Vector3 diff = (v - position).normalized;
        projectile.GetComponent<MeshRenderer>().material = materialProjectile;
        projectile.GetComponent<Light>().color = ColorProjectile;
        projectile.GetComponent<Projectile>().ShootBullet(position, diff, 7, damagePerImpact, 20);
        projectile.GetComponent<NetworkObject>().Spawn();
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

    }

    protected override void UpdateStateAttack()
    {
        if (CheckIfItsFarAwayPlayer())
        {
            ChangeState(StateOfEnemy.FOLLOWING);
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
                ChangeState(StateOfEnemy.FOLLOWING);
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
