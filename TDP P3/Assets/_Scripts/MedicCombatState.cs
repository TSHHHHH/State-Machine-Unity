using UnityEngine;

public class MedicCombatState : CombatState
{
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

        if (enemyManager.HandleDetection())
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
}
