using Cinemachine.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    //      StateMachine and Basic Stats
    [SerializeField]
    protected GameObject parentOfPoints;
    protected float damagePerImpact;
    protected float maxHealth;
    protected float currentHealth;
    protected StateOfEnemy currentState;
    protected List<Vector3> randomPositions = new List<Vector3>();
    protected float RangeAttack;
    protected float velocity;
    
    //      Pathfinding
    protected Rigidbody rb; 
    protected NavMeshPath currentPath;
    protected int navmeshIndexPosition;

    //      Enemy vision
    [SerializeField]
    [Range(0f, 360f)]
    public float angle;
    protected Transform playerRef;
    protected int indexCurrentPointAlert;
    [SerializeField]
    protected LayerMask targetMask;
    [SerializeField]
    protected LayerMask obstructionMask;
    [NonSerialized]
    public bool canSeePlayer,check, Attacking;

    protected void SetRandomPostions()
    {
        if(targetMask == null)
        {
            targetMask |= (1 << LayerMask.NameToLayer("Target"));
        }
        if (obstructionMask == null)
        {
            obstructionMask |= (1 << LayerMask.NameToLayer("Target"));
            obstructionMask |= (1 << LayerMask.NameToLayer("Obstruction"));
        }
        for (int a  = 0; a < parentOfPoints.transform.childCount; a++) { 
            randomPositions.Add(parentOfPoints.transform.GetChild(a).position);
        }
    }

    private IEnumerator FOVRoutine(Transform transform)
    {
        check = true;
        FieldOfViewCheck(transform);
        if (canSeePlayer)
        {
            playerRef = transform;
            currentState = StateOfEnemy.FOLLOWING;
        }
        while (canSeePlayer)
        {
            yield return new WaitForSeconds(0.2f);
            FieldOfViewCheck(transform);
        }
        check = false;  
    }
    private void OnTriggerStay(Collider other)
    {
        if (!check)
        {
            //Debug.Log(other.transform.name);
            StartCoroutine(FOVRoutine(other.transform));
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        canSeePlayer = false;
    }
    private void FieldOfViewCheck(Transform transformTarget)
    {
        Vector3 directionToTarget = (transformTarget.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
        {
            RaycastHit raycastInfo;
            if (Physics.Raycast(transform.position, directionToTarget, out raycastInfo, transform.GetChild(1).GetComponent<SphereCollider>().radius, obstructionMask))
            {
                canSeePlayer = raycastInfo.transform.gameObject.layer == LayerMask.NameToLayer("Target");
            }
            else canSeePlayer = false;
        }
        else
        {
            canSeePlayer = false;
        }

    }
}

public enum StateOfEnemy
{
    PATROL, ATTACK, FOLLOWING
}