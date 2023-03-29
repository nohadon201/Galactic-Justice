using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Thraaxian : EnemyBehaviour
{
    private float AngleAttack;
    private Vector3 PositionAttack;
    private bool iker;
    void Awake()
    {
        //  Thraaxian custom values
        AngleAttack = Random.Range(0, 360);  
        //  Thraaxian unique values
        filter.agentTypeID = 0;
        filter.areaMask = 7;
        velocity = 10.0f;
        currentPath = new();
        damagePerImpact = 7f;
        maxHealth = 5;
        currentHealth = maxHealth;
        RangeAttack = 12;
        //  Generic Enemy default values
        SetRandomPostions();
        navmeshIndexPosition = 0;
        rb = GetComponent<Rigidbody>();
        
    }

    private void Start()
    {
        indexCurrentPointAlert = UnityEngine.Random.Range(0, randomPositions.Count);
        CalculatePath(randomPositions[indexCurrentPointAlert]);
        ChangeState(StateOfEnemy.PATROL);
    }
    /*
     * ################################### Range Attack ##############################################
     */
    private bool CheckIfItsClosePlayer()
    {
        Vector3 v = (PositionAttack - transform.position);
        return (Mathf.Abs(v.x) <= 1) && (Mathf.Abs(v.z) <= 1 );

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
            this.AngleAttack = Random.Range(0, 360);
        }
        navmeshIndexPosition = 0;
    }
    public void CalculatePositionAroundPlayer()
    {
        if(currentState != StateOfEnemy.PATROL)
        {
            Vector3 v = new Vector3(Mathf.Cos(AngleAttack) * RangeAttack, 0, Mathf.Sin(AngleAttack) * RangeAttack);
            this.PositionAttack = playerRef.position + v;
            CalculatePath(playerRef.position + v);
        }
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
        GameObject pool = GeneralPool.Instance.poolThraaxian;
        for (int a = 0; a < pool.transform.childCount; a++)
        {
            if (!pool.transform.GetChild(a).gameObject.activeSelf)
            {
                
                Vector3 position = transform.position + (transform.forward * 2.5f);
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
        Debug.Log("OnPlayerSeen");
        switch (currentState)
        {
            case StateOfEnemy.PATROL:
                ChangeState(StateOfEnemy.FOLLOWING);   
                break;
            case StateOfEnemy.FOLLOWING:
                if (forgivePlayer != null)
                {
                    if (!iker) {

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

    protected override void InitStatePatrol()
    {
        Debug.Log("Patrol"); 
        if (findPlayer != null)
        {
            Debug.Log("NO ENTRA POR QUE SOY UN DESGRACIADO QUE NO SABE PROGRAMAR");
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
        Debug.Log("FIXED UPDATE PATROL");
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
        
        AngleAttack = UnityEngine.Random.Range(0, 360);
    }

    //          ATTACK

    protected override void InitStateAttack()
    {
        Debug.Log("Attack");
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        attack = StartCoroutine(AttackingCoroutine());
        if (forgivePlayer != null)
        {
            StopCoroutine(forgivePlayer);
            print("iker rencoroso");
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
        Debug.Log("QUIRAXIAN ATTACKSTOP");
        StopCoroutine(attack);
    }

    protected override IEnumerator FindPlayer()
    {
        while (true)
        {
            CalculatePositionAroundPlayer();
            yield return new WaitForSeconds(1);
        }
    }
    protected override IEnumerator ForgivePlayer()
    {
        PlayerForgive = true;
        iker = true;
        yield return new WaitForSeconds(0.2f);
        iker = false;
        Debug.Log("forgivestatrt");
        yield return new WaitForSeconds(3);
        Debug.Log("forgiveend");
        ChangeState(StateOfEnemy.PATROL);
        PlayerForgive = false;
    }
    
    protected override void OnPlayerAway()
    {
        switch (currentState)
        {
            case StateOfEnemy.PATROL:
                break;
            case StateOfEnemy.ATTACK:
                Debug.Log("ATTACK ONPLAYER AWAY");
                Debug.Log(PlayerForgive);
                if (!PlayerForgive)
                {
                    forgivePlayer = StartCoroutine(ForgivePlayer());
                }
                ChangeState(StateOfEnemy.FOLLOWING);
                break;
            case StateOfEnemy.FOLLOWING:
                Debug.Log("FOLLOWING ON PLAYER AWAY");
                if (!PlayerForgive)
                {
                    forgivePlayer = StartCoroutine(ForgivePlayer());
                }
                break;
            default:
                break;
        }
    }
}
