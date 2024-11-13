using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    [Header("References")]
    private NavMeshAgent navMeshAgent;

    private EnemyStats enemyStats;
    private FieldOfView fov;
    private EnemyWeaponManager weaponManager;

    private PlayerManager playerManager;

    [Header("FSM Vars")]
    [SerializeField] private State startState;
    private State currentState;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        enemyStats = GetComponent<EnemyStats>();
        fov = GetComponent<FieldOfView>();
        weaponManager = GetComponent<EnemyWeaponManager>();
    }

    private void Start()
    {
        playerManager = ServiceLocater.GetService<PlayerManager>();

        currentState = startState;
        currentState.OnFSMStateEnter(this, enemyStats);
    }

    private void Update()
    {
        HandleFSM();
    }

    private void HandleFSM()
    {
        if (currentState != null)
        {
            State nextState = currentState.Tick(this, enemyStats);

            if (nextState != null)
            {
                SwitchToNextState(nextState);
            }
        }
    }

    public void SwitchToNextState(State nextState)
    {
        if (currentState != nextState)
        {
            currentState.OnFSMStateExit(this, enemyStats);
            currentState = nextState;
            currentState.OnFSMStateEnter(this, enemyStats);
        }
        else
        {
            currentState = nextState;
        }
    }

    #region Nav Agent

    public void EnableNavAgent()
    {
        if (navMeshAgent.enabled == false)
            navMeshAgent.enabled = true;

        navMeshAgent.speed = enemyStats.moveSpeed;
    }

    public void DisableNavAgent()
    {
        if (navMeshAgent.enabled == true)
            navMeshAgent.enabled = false;

        StopNavAgent();
    }
    internal void SetNavAgentSpeed(float newSpeed)
    {
        navMeshAgent.speed = newSpeed;
    }

    public void StopNavAgent()
    {
        navMeshAgent.velocity = Vector3.zero;
    }

    public void UpdateNavAgentDestination(Vector3 destination)
    {
        navMeshAgent.destination = destination;
    }

    public void RotateWithNavAgent()
    {
        if (navMeshAgent.velocity != Vector3.zero)
        {
            float angle = Mathf.Atan2(navMeshAgent.velocity.x, navMeshAgent.velocity.y) * Mathf.Rad2Deg;

            // lerp the rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, -angle), Time.deltaTime * enemyStats.rotationSpeed * enemyStats.rotationMultiplier);
        }
    }

    #endregion Nav Agent

    public bool HandleDetection()
    {
        GameObject playerObj = playerManager.gameObject;

        if (fov.IsTargetInFieldOfView(playerObj.transform.position))
        {
            enemyStats.currentTarget = playerObj.transform;

            return true;
        }

        return false;
    }

    public bool CheckIfInWeaponFireRange()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, enemyStats.currentTarget.position);

        // Debug.Log("Distance to player: " + distanceToPlayer);
        // Debug.Log(weaponManager.weaponData.range);

        if (distanceToPlayer <= weaponManager.weaponData.range)
        {
            return true;
        }

        // Debug.Log("Not in range");

        return false;
    }

    public void Fire()
    {
        weaponManager.Fire();
    }
}