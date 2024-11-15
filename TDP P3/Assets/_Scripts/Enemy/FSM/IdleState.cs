using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class IdleState : State
{
    [Header("FSM Vars")]
    [SerializeField] private PursueState pursueState;
    [SerializeField] private FleeState fleeState;
    [SerializeField] private DeathState deathState;

    [Header("Idle State Vars")]
    [SerializeField] private float rotationAngle = 30f;
    private float currentRotationAngle = 0f;
    private bool isRotatingRight = true;

    public override void OnFSMStateEnter(EnemyManager enemyManager, EnemyStats enemyStats)
    {
        // enemyStats.currentTarget = null;
    }

    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats)
    {
        if (enemyStats.isDead)
        {
            return deathState;
        }

        if (!enemyStats.isHealth())
        {
            return fleeState;
        }

        // IdleRotation(enemyStats);

        if (enemyManager.HandleDetection() || enemyStats.currentTarget != null)
        {
            return pursueState;
        }
        else
        {
            return this;
        }
    }

    protected void IdleRotation(EnemyStats enemyStats)
    {
        // check if rotation angle is 0, if so, return
        if (rotationAngle <= 0f)
        {
            return;
        }

        if (isRotatingRight)
        {
            if(currentRotationAngle < rotationAngle)
            {
                currentRotationAngle += enemyStats.rotationSpeed * enemyStats.rotationMultiplier * Time.deltaTime;

                // lerp rotation
                enemyStats.transform.Rotate(Vector3.forward, enemyStats.rotationSpeed * enemyStats.rotationMultiplier * Time.deltaTime);
            }
            else
            {
                isRotatingRight = false;
                currentRotationAngle = 0f;
            }
        }
        else
        {
            if(currentRotationAngle < rotationAngle)
            {
                currentRotationAngle += enemyStats.rotationSpeed * enemyStats.rotationMultiplier * Time.deltaTime;

                enemyStats.transform.Rotate(Vector3.forward, -enemyStats.rotationSpeed * enemyStats.rotationMultiplier * Time.deltaTime);
            }
            else
            {
                isRotatingRight = true;
                currentRotationAngle = 0f;
            }
        }
    }

    public override void OnFSMStateExit(EnemyManager enemyManager, EnemyStats enemyStats)
    {
    }
}