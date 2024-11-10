using System;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    [Header("FSM Vars")]
    [SerializeField] private PursueState pursueState;

    [SerializeField] private List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    private bool patrolForward = true;

    public override void OnFSMStateEnter(EnemyManager enemyManager, EnemyStats enemyStats)
    {
        // enable nav agent
        enemyManager.EnableNavAgent();

        // set destination to first waypoint
        enemyManager.UpdateNavAgentDestination(waypoints[currentWaypointIndex].position);
    }

    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats)
    {
        HandlePatrol(enemyManager);
        enemyManager.RotateWithNavAgent();

        // detect player
        if (enemyStats.currentTarget != null || enemyManager.HandleDetection(enemyStats))
        {
            return pursueState;
        }

        return this;
    }

    #region Patrol Movement

    private void HandlePatrol(EnemyManager enemyManager)
    {
        if (ReachedWaypoint(enemyManager))
        {
            if (patrolForward)
            {
                currentWaypointIndex++;
            }
            else
            {
                currentWaypointIndex--;
            }

            if (currentWaypointIndex >= waypoints.Count)
            {
                currentWaypointIndex = waypoints.Count - 2;
                patrolForward = false;
            }
            else if (currentWaypointIndex < 0)
            {
                currentWaypointIndex = 1;
                patrolForward = true;
            }

            enemyManager.UpdateNavAgentDestination(waypoints[currentWaypointIndex].position);
        }
    }

    private bool ReachedWaypoint(EnemyManager enemyManager)
    {
        float distanceToWaypoint = Vector3.Distance(enemyManager.transform.position, waypoints[currentWaypointIndex].position);

        if (distanceToWaypoint < 1f)
        {
            return true;
        }

        return false;
    }

    #endregion

    public override void OnFSMStateExit(EnemyManager enemyManager, EnemyStats enemyStats)
    {
        
    }
}
