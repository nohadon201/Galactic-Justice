using Cinemachine.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.VersionControl.Asset;

public abstract class EnemyBehaviour : MonoBehaviour
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
    
    protected NavMeshQueryFilter filter;
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

    private Coroutine checkPlayerCoroutine;
    protected Coroutine attack, findPlayer;
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

    private IEnumerator FOVRoutine(Transform transformPlayer)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (FieldOfViewCheck(transformPlayer))
            {
                Debug.Log(transformPlayer.name);
                playerRef = transformPlayer;
                OnPlayerSeen();
            }
        }
    }
    protected abstract IEnumerator FindPlayer();
    private void Update()
    {
        UpdateState(currentState);
    }

    private void FixedUpdate()
    {
        FixedUpdateState(currentState);
    }

    private void OnTriggerEnter(Collider other)
    {
        checkPlayerCoroutine = StartCoroutine(FOVRoutine(other.transform));        
    }
    private void OnTriggerExit(Collider other)
    {
        StopCoroutine(checkPlayerCoroutine);
    }
    private bool FieldOfViewCheck(Transform transformTarget)
    {
        Vector3 directionToTarget = (transformTarget.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
        {
            RaycastHit raycastInfo;
            if (Physics.Raycast(transform.position, directionToTarget, out raycastInfo, transform.GetChild(1).GetComponent<SphereCollider>().radius, obstructionMask))
            {
                return raycastInfo.transform.gameObject.layer == LayerMask.NameToLayer("Target");
            }
        }
        return false;
    }

    protected void ChangeState(StateOfEnemy newState)
    {
        if (newState == currentState)
            return;

        ExitState(currentState);
        InitState(newState);
    }

    private void InitState(StateOfEnemy initState)
    {
        currentState = initState;

        switch (currentState)
        {
            case StateOfEnemy.PATROL:
                InitStatePatrol();
                break;
            case StateOfEnemy.FOLLOWING:
                InitStateFollowing();
                break;
            case StateOfEnemy.ATTACK:
                InitStateAttack();
                break;
            default:
                break;
        }
    }

    private void UpdateState(StateOfEnemy updateState)
    {
        switch (updateState)
        {
            case StateOfEnemy.PATROL:
                UpdateStatePatrol();
                break;
            case StateOfEnemy.FOLLOWING:
                UpdateStateFollowing();
                break;
            case StateOfEnemy.ATTACK:
                UpdateStateAttack();
                break;
            default:
                break;
        }
    }

    private void FixedUpdateState(StateOfEnemy updateState)
    {
        switch (updateState)
        {
            case StateOfEnemy.PATROL:
                FixedUpdateStatePatrol();
                break;
            case StateOfEnemy.FOLLOWING:
                FixedUpdateStateFollowing();
                break;
            case StateOfEnemy.ATTACK:
                FixedUpdateStateAttack();
                break;
            default:
                break;
        }
    }

    private void ExitState(StateOfEnemy exitState)
    {
        switch (exitState)
        {
            case StateOfEnemy.PATROL:
                ExitStatePatrol();
                break;
            case StateOfEnemy.FOLLOWING:
                ExitStateFollowing();
                break;
            case StateOfEnemy.ATTACK:
                ExitStateAttack();
                break;
            default:
                break;
        }
    }


    protected abstract void OnPlayerSeen();
    protected abstract void InitStatePatrol();
    protected abstract void UpdateStatePatrol();
    protected abstract void FixedUpdateStatePatrol();
    protected abstract void ExitStatePatrol();
    protected abstract void InitStateFollowing();
    protected abstract void UpdateStateFollowing();
    protected abstract void FixedUpdateStateFollowing();
    protected abstract void ExitStateFollowing();
    protected abstract void InitStateAttack();
    protected abstract void UpdateStateAttack();
    protected abstract void FixedUpdateStateAttack();
    protected abstract void ExitStateAttack();

}

public enum StateOfEnemy
{
    PATROL, ATTACK, FOLLOWING
}