using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatState : State
{
    [Header("FSM Vars")]
    [SerializeField] protected PursueState pursueState;
    [SerializeField] protected FleeState fleeState;
    [SerializeField] protected DeathState deathState;

    public override void OnFSMStateEnter(EnemyManager enemyManager, EnemyStats enemyStats)
    {
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

        RotateToTarget(enemyManager, enemyStats);

        if(enemyManager.HandleDetection())
        {
            enemyManager.Fire();
        }

        // if the player is too far away, return to pursue state
        if (!enemyManager.CheckIfInWeaponFireRange())
        {
            return pursueState;
        }
        else
        {
            // walk around the player
            StrafeMovement(enemyManager, enemyStats);
        }

        return this;
    }

    protected void RotateToTarget(EnemyManager enemyManager, EnemyStats enemyStats)
    {
        // compute direction to look at
        Vector2 playerDir = new Vector2(enemyStats.currentTarget.position.x - enemyManager.transform.position.x, enemyStats.currentTarget.position.y - enemyManager.transform.position.y);

        // compute rotation
        float angle = Mathf.Atan2(playerDir.y, playerDir.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));

        // apply rotation
        enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, targetRotation, enemyStats.rotationSpeed * enemyStats.rotationMultiplier * Time.deltaTime);
    }

    protected virtual void StrafeMovement(EnemyManager enemyManager, EnemyStats enemyStats)
    {
        // the default enemy does not strafe, so the base method does nothing
    }

    public override void OnFSMStateExit(EnemyManager enemyManager, EnemyStats enemyStats)
    {
    }
}