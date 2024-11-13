using UnityEngine;

public class RusherCombatState : CombatState
{
    [Header("Strafe Vars")]
    [SerializeField] private float stopDistance = 1.5f;

    protected override void StrafeMovement(EnemyManager enemyManager, EnemyStats enemyStats)
    {
        enemyManager.EnableNavAgent();

        float distance = Vector3.Distance(enemyManager.transform.position, enemyStats.currentTarget.position);

        if (distance < stopDistance)
        {
            enemyManager.DisableNavAgent();
        }
        else
        {
            enemyManager.UpdateNavAgentDestination(enemyStats.currentTarget.position);
        }
    }
}
