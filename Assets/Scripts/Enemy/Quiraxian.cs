using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Quiraxian : EnemyBehaviour
{

    void Awake()
    {
        //  Pyrognathian unique values
        filter.agentTypeID = 0;
        filter.areaMask = 7;
        velocity = 10.0f;
        currentPath = new();
        damagePerImpact = 20f;
        maxHealth = 20;
        currentHealth = maxHealth;
        RangeAttack = 9;
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
        Vector3 v = (playerRef.transform.position - transform.position);
        return (v.x > RangeAttack || v.x < -RangeAttack) || (v.z > RangeAttack || v.z < -RangeAttack);
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
            Shoot(playerRef.transform.position);
            yield return new WaitForSeconds(1f);
        }
        
    }
    public void Shoot(Vector3 v)
    {
        GameObject pool = GeneralPool.Instance.poolQuiraxian;
        for (int a = 0; a < pool.transform.childCount; a++)
        {
            if (!pool.transform.GetChild(a).gameObject.activeSelf)
            {
                Vector3 position = transform.position + (transform.forward * 2);
                Vector3 diff = (v - position).normalized;
                pool.transform.GetChild(a).gameObject.SetActive(true);
                pool.transform.GetChild(a).gameObject.GetComponent<Projectile>().ShootBullet(position, diff, 7, damagePerImpact, 20);
                break;
            }
        }
    }
    /*
     * ################################### State Machine ##############################################
     */
    //          PATROL
    protected override void OnPlayerSeen()
    {
        if(forgivePlayer != null)
        {
            StopCoroutine(forgivePlayer);
        }
        ChangeState(StateOfEnemy.FOLLOWING);
    }

    protected override void InitStatePatrol()
    {
        Debug.Log("Patrol");
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
        Debug.Log("Following");
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
        Debug.Log("Attack");
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        attack = StartCoroutine(AttackingCoroutine());
    }

    protected override void UpdateStateAttack()
    {
        if(CheckIfItsFarAwayPlayer())
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
        yield return new WaitForSeconds(10);
        ChangeState(StateOfEnemy.PATROL);
    }
    protected override void OnPlayerAway()
    {
        forgivePlayer = StartCoroutine(ForgivePlayer());
    }
}
