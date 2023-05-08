using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBehaviour : NetworkBehaviour
{
    /**
     * ###################################### EVENTS ################################ 
     */
    [SerializeField] private GameEvent OnEnemyDeathEvent;
    [SerializeField] private GameEvent OnPlayerSeenEvent;
    [SerializeField] private GameEvent<float> OnDamageReceivedEvent;

    /**
     * ###################################### State Machine ################################ 
     */
    protected float damagePerImpact;
    protected float maxHealth;
    protected float currentHealth;
    [SerializeField]
    protected StateOfEnemy currentState;
    public List<Vector3> randomPositions = new List<Vector3>();
    protected float RangeAttack;
    public float velocity;
    protected bool PlayerForgive;
    public float CooldownAttack;

    /**
     * ###################################### Pathfinding ################################ 
     */
    protected NavMeshQueryFilter filter;
    protected Rigidbody rb; 
    protected NavMeshPath currentPath;
    protected int navmeshIndexPosition;

    /**
      * ###################################### Piercing ################################ 
      */
    [SerializeField]
    private Transform piercingPoint;
    public Transform PiercingPoint => piercingPoint;

    /**
      * ###################################### Enemy Vision ################################ 
      */
    [SerializeField]
    [Range(0f, 360f)]
    public float angle;
    [SerializeField]    
    public Transform playerRef;
    protected int indexCurrentPointAlert;
    [SerializeField]
    protected LayerMask targetMask;
    [SerializeField]
    protected LayerMask obstructionMask;
    private Coroutine checkPlayerCoroutine;
    protected Coroutine attack, findPlayer, forgivePlayer, RunAwayPlayer;

    protected virtual void Awake()
    {
        if (IsServer)
        {
            GetComponent<NetworkObject>().Spawn();
        }
          
    }
    protected void SetLayers()
    {
        
        targetMask |= (1 << LayerMask.NameToLayer("Target"));
        
        obstructionMask |= (1 << LayerMask.NameToLayer("Target"));
        obstructionMask |= (1 << LayerMask.NameToLayer("Obstruction"));

    }
    
    private void Update()
    {
        if (!IsServer) return;
        UpdateState(currentState);
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;
        OnEnemyFall();
        FixedUpdateState(currentState);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if(other.transform.tag == "Player")
        {
            playerRef = other.transform;
            checkPlayerCoroutine = StartCoroutine(FOVRoutine(other.transform));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;
        if (other.transform.tag == "Player")
        {
            StopCoroutine(checkPlayerCoroutine);
            OnPlayerAway();
        }
    }
    /**
     * ###################################### Patrol Of Enemy ################################ 
     */
    private IEnumerator FOVRoutine(Transform transformPlayer)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (FieldOfViewCheck(transformPlayer))
            {
                if (playerRef == null) OnPlayerSeenEvent?.Raise();
                OnPlayerSeen();
            }else 
            {
                OnPlayerAway();
            }
        }
    }
    private bool FieldOfViewCheck(Transform transformTarget)
    {
        Vector3 directionToTarget = (transformTarget.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
        {
            RaycastHit raycastInfo;
            if (Physics.Raycast(transform.position, directionToTarget, out raycastInfo, transform.GetChild(1).GetComponent<SphereCollider>().radius, obstructionMask))
            {
                return raycastInfo.transform.gameObject.layer == LayerMask.NameToLayer("Target") && raycastInfo.transform.tag == "Player";
            }
        }
        return false;
    }
    /**
     * ###################################### Movement ################################ 
     */
    protected bool KeepGoingPatrolPoint()
    {
        Vector3 v = (randomPositions[indexCurrentPointAlert] - transform.position);
        if (Mathf.Abs(v.x) <= .1 && Mathf.Abs(v.z) <= .1)
        {
            int a = indexCurrentPointAlert;
            while (a == indexCurrentPointAlert)
            {
                a = UnityEngine.Random.Range(0, randomPositions.Count);
            }
            indexCurrentPointAlert = a;
            return true;
        }
        return false;
    }
    protected void setTheVelocity()
    {
        if (navmeshIndexPosition < currentPath.corners.Length)
        {
            float xCoor = currentPath.corners[navmeshIndexPosition].x - transform.position.x;
            float zCoor = currentPath.corners[navmeshIndexPosition].z - transform.position.z;
            Vector3 d = (new Vector3(xCoor, 0, zCoor).normalized * velocity);
            d.y += rb.velocity.y;
            rb.velocity = d;
            Vector3 v = currentPath.corners[navmeshIndexPosition] - transform.position;
            if ((v.x < 0.1 && v.x > -0.1) && (v.z < 0.1 && v.z > -0.1))
            {
                navmeshIndexPosition++;
            }
        }
    }
    public void GetHit(float damage)
    {
        OnDamageReceivedEvent?.Raise(damage);
        Debug.Log("DAMAGE! " + damage);
        currentHealth-= damage; 
        if(currentHealth <= 0)
        {
            OnEnemyDeathEvent?.Raise();
            this.gameObject.SetActive(false);
            DieClientRpc();
        }
    }
    [ClientRpc]
    public void DieClientRpc()
    {
        this.gameObject.SetActive(false);
    }
    /**
     * ###################################### State Machine ################################ 
     */
    public void OnEnemyFall()
    {
        if(transform.position.y < -20)
        {
            gameObject.SetActive(false);
            OnEnemyDeathEvent?.Raise(); 
        }
    }
    public bool IsInPatrol()
    {
        return currentState == StateOfEnemy.PATROL;
    }
    public void ChangeState(StateOfEnemy newState)
    {
        Debug.Log("CHANGE STATE FROM: "+ currentState.ToString() + " TO "+ newState.ToString());  
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
                Debug.Log("Init PATROL");
                InitStatePatrol();
                break;
            case StateOfEnemy.FOLLOWING:
                Debug.Log("Init FOLLOWING");
                InitStateFollowing();
                break;
            case StateOfEnemy.ATTACK:
                Debug.Log("Init ATTACK");
                InitStateAttack();
                break;
            case StateOfEnemy.CRAZY:
                Debug.Log("Init CRAZY");
                InitStateCrazy();
                break;
            case StateOfEnemy.TERRIFIED:
                Debug.Log("Init TERRIFIED");
                InitStateTerrified();
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
            case StateOfEnemy.CRAZY:    
                UpdateStateCrazy();
                break;
            case StateOfEnemy.TERRIFIED:
                UpdateStateTerrified();
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
            case StateOfEnemy.CRAZY:
                FixedUpdateStateCrazy();
                break;
            case StateOfEnemy.TERRIFIED:
                FixedUpdateStateTerrified();
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
            case StateOfEnemy.CRAZY:
                ExitStateCrazy();
                break;
            case StateOfEnemy.TERRIFIED:
                ExitStateTerrified();
                break;
            default:
                break;
        }
    }
    protected IEnumerator RunAwayFromPlayer()
    {
        currentPath.ClearCorners();
        NavMesh.CalculatePath(transform.position, RunAway(), filter, currentPath);
        navmeshIndexPosition = 0;
        while (currentState == StateOfEnemy.TERRIFIED)
        {
            if(10f > Vector3.Distance(transform.position, playerRef.transform.position))
            {
                currentPath.ClearCorners();
                NavMesh.CalculatePath(transform.position, RunAway(), filter, currentPath);
                navmeshIndexPosition = 0;
            }    
            yield return new WaitForSeconds(1f);
        }
    }
    private Vector3 RunAway()
    {
        Vector3 diff = transform.position - playerRef.position;
        diff.Normalize();
        diff.y = 0;
        return playerRef.transform.position + (diff * 30);

    }
    /**
     * ###################################### Abstract Functions ################################ 
     */
    protected abstract IEnumerator FindPlayer();
    protected abstract IEnumerator ForgivePlayer();
    protected abstract void OnPlayerSeen();
    protected abstract void OnPlayerAway();
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
    protected abstract void InitStateCrazy();
    protected abstract void UpdateStateCrazy();
    protected abstract void FixedUpdateStateCrazy();
    protected abstract void ExitStateCrazy();
    protected abstract void InitStateTerrified();
    protected abstract void UpdateStateTerrified();
    protected abstract void FixedUpdateStateTerrified();
    protected abstract void ExitStateTerrified();

}

public enum StateOfEnemy
{
    PATROL, ATTACK, FOLLOWING, STUNED, CRAZY, TERRIFIED
}