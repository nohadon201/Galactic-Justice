using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Quiraxian : EnemyBehaviour
{
    void Awake()
    {
        //  Pyrognathian unique values
        velocity = 10.0f;
        currentPath = new();
        damagePerImpact = 20f;
        maxHealth = 20;
        currentHealth = maxHealth;
        RangeAttack = 15;
        //  Generic Enemy default values
        SetRandomPostions();
        navmeshIndexPosition = 0;
        currentState = StateOfEnemy.PATROL;
        Attacking = false;
        check = false;
        canSeePlayer = false;
        rb = GetComponent<Rigidbody>(); 
    }

    private void Start()
    {
        indexCurrentPointAlert = Random.Range(0, randomPositions.Count);
        CalculatePath(randomPositions[indexCurrentPointAlert]);
    }
    /*
     * ################################### StateMachine ##############################################
     */
    void Update()
    {
        //if(currentState == StateOfEnemy.FOLLOWING)
        //{
        //    Debug.Log("Velocity: " + rb.velocity);
        //}

        switch(currentState) {
            case StateOfEnemy.PATROL:
                Vector3 forward = randomPositions[indexCurrentPointAlert] - transform.position;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forward), 0.2f );    
                break;
            case StateOfEnemy.FOLLOWING:
            case StateOfEnemy.ATTACK:
                Vector3 forward2 = playerRef.position - transform.position;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forward2),0.2f);
                break;
        }

    }
    private void FixedUpdate()
    {
        CoreStateMachine();
        if (navmeshIndexPosition < currentPath.corners.Length)
        {
            if (Vector3.Distance(transform.position, currentPath.corners[navmeshIndexPosition]) <= 2f)
            {
                navmeshIndexPosition++;
            }
        }
    }
    private void LateUpdate()
    {
        if (navmeshIndexPosition < currentPath.corners.Length)
        {
            //Debug.Log(currentPath.corners[0]);
            rb.velocity = setTheVelocity();
            

        }
        else
        {
            Debug.Log("iiiiiiiiiiiiiiiiiii");
        }
    }
    private void CoreStateMachine()
    {
        switch (currentState)
        {
            case StateOfEnemy.PATROL:
                if (KeepGoingPatrolPoint())
                {
                    CalculatePath(randomPositions[indexCurrentPointAlert]);
                }
                Debug.Log("PATROL");
                break;
            case StateOfEnemy.FOLLOWING:
                if (CheckIfItsClosePlayer())
                {
                    currentState = StateOfEnemy.ATTACK;
                    StartCoroutine(AttackingCoroutine());
                    break;
                }
                Debug.Log("FOLLOWING");
                CalculatePath(playerRef.position);
                break;
            case StateOfEnemy.ATTACK:
                if (CheckIfItsFarAwayPlayer())
                {
                    currentState = StateOfEnemy.FOLLOWING;
                    Attacking = false;
                    break;
                }
                Debug.Log("ATTACKS");
                CalculatePath(transform.position);
                break;
        }
    }
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
     * ################################### Movement with Navmesh ##############################################
     */
    private void CalculatePath(Vector3 position)
    {
        currentPath.ClearCorners();
        Debug.Log(position);
        if (!NavMesh.CalculatePath(transform.position, position, NavMesh.AllAreas, currentPath))
        {
            Debug.Log("NO");
        }
        navmeshIndexPosition = 0;
    }
    private bool KeepGoingPatrolPoint()
    {
        Vector3 v = (randomPositions[indexCurrentPointAlert] - transform.position);
        if (v.x < 5 && v.z < 5)
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
        Vector3 direction = (currentPath.corners[navmeshIndexPosition] - transform.position).normalized;
        return direction * velocity;
    }
    /*
     * ################################### Attack ##############################################
     */
    private IEnumerator AttackingCoroutine()
    {
        Attacking = true;
        while (Attacking)
        {
            Shoot(playerRef.transform.position);
            yield return new WaitForSeconds(1);
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
}
