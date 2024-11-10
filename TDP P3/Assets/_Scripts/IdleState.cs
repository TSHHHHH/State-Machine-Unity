using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class IdleState : State
{
    [Header("FSM Vars")]
    [SerializeField] private PursueState pursueState;

    [SerializeField] private DeathState deathState;

    public override void OnFSMStateEnter(EnemyManager enemyManager, EnemyStats enemyStats)
    {
        enemyStats.currentTarget = null;
    }

    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats)
    {
        if (enemyStats.isDead)
        {
            return deathState;
        }

        if (enemyManager.HandleDetection(enemyStats))
        {
            Debug.Log("Player Detected");

            return pursueState;
        }
        else if (enemyStats.currentTarget != null)
        {
            return pursueState;
        }
        else
        {
            return this;
        }
    }

    public override void OnFSMStateExit(EnemyManager enemyManager, EnemyStats enemyStats)
    {
    }
}