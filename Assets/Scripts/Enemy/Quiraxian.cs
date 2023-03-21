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
        RangeAttack = 10;
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
        Debug.Log(position);
        if (!NavMesh.CalculatePath(transform.position, position, filter, currentPath))
        {
            Debug.Log("No Path finded");
        }
        navmeshIndexPosition = 0;
    }
    private bool KeepGoingPatrolPoint()
    {
        Vector3 v = (randomPositions[indexCurrentPointAlert] - transform.position);
        if (Mathf.Abs(v.x) <= .1 && Mathf.Abs(v.z) <= .1)
        {
            int a = indexCurrentPointAlert;
            while (a == indexCurrentPointAlert)
            {
                a = Random.Range(0, randomPositions.Count);
            }
            indexCurrentPointAlert = a;
            return true;
        }
        return false;
    }
    private Vector3 setTheVelocity()
    {
        float x = currentPath.corners[navmeshIndexPosition].x - transform.position.x;
        float z = currentPath.corners[navmeshIndexPosition].z - transform.position.z;
        Vector3 direction = new Vector3(x, 0, z);
        return direction.normalized * velocity;
    }
    /*
     * ################################### Attack ##############################################
     */
    private IEnumerator AttackingCoroutine()
    {
        while (true)
        {
            Shoot(playerRef.transform.position);
            yield return new WaitForSeconds(2.5f);
        }
        
    }
    public void Shoot(Vector3 v)
    {
        GameObject pool = GeneralPool.Instance.poolThraaxian;
        for (int a = 0; a < pool.transform.childCount; a++)
        {
            if (!pool.transform.GetChild(a).gameObject.activeSelf)
            {
                Vector3 diff = (v - transform.position).normalized;
                Vector3 position = transform.position + (transform.forward * 3);
                pool.transform.GetChild(a).gameObject.SetActive(true);
                pool.transform.GetChild(a).gameObject.GetComponent<Projectile>().ShootBullet(position, diff, 7, damagePerImpact);
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
        if (navmeshIndexPosition < currentPath.corners.Length)
        {
            Vector3 v = currentPath.corners[navmeshIndexPosition] - transform.position;
            if ((v.x < 0.1 && v.x > -0.1) && (v.z < 0.1 && v.z > -0.1))
            {
                navmeshIndexPosition++;
            }
            rb.velocity = setTheVelocity();
        }
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
        if (navmeshIndexPosition < currentPath.corners.Length)
        {
            Vector3 v = currentPath.corners[navmeshIndexPosition] - transform.position;
            if ((v.x < 0.1 && v.x > -0.1) && (v.z < 0.1 && v.z > -0.1))
            {
                navmeshIndexPosition++;
            }
            rb.velocity = setTheVelocity();
        }
        Debug.Log(rb.velocity);
        if (CheckIfItsClosePlayer())
        {
            ChangeState(StateOfEnemy.ATTACK);
        }
    }

    protected override void ExitStateFollowing()
    {
        StopCoroutine(FindPlayer());
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
}
