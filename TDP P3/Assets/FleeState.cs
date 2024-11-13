using UnityEngine;
using UnityEngine.AI;

public class FleeState : State
{
    [Header("FSM Vars")]
    [SerializeField] private CombatState combatState;
    [SerializeField] private DeathState deathState;

    [Header("Flee Vars")]
    [SerializeField] private float moveSpeedMultiplier = 0.5f;
    [SerializeField] private float fleeDistance = 10f;
    [SerializeField] private float obstacleDetectionRadius = 1f;
    [SerializeField] private int maxAttempts = 50;

    [SerializeField] private float medicBagDetectionRadius = 5f;

    [Header("Debug Settings")]
    [SerializeField] private bool isDebugMode = false;

    public override void OnFSMStateEnter(EnemyManager enemyManager, EnemyStats enemyStats)
    {
        enemyManager.EnableNavAgent();

        enemyManager.SetNavAgentSpeed(enemyStats.moveSpeed * moveSpeedMultiplier);
        
    }

    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats)
    {
        if (enemyStats.isDead)
        {
            return deathState;
        }

        // if the enemy is healthy again, return to combat state
        if (enemyStats.isHealth())
        {
            return combatState;
        }

        enemyManager.RotateWithNavAgent();

        if (LookForMedicBag(enemyManager))
        {
            return this;
        }
        else
        {
            Vector3 fleeDirection = (enemyManager.gameObject.transform.position - enemyStats.currentTarget.transform.position).normalized;

            for (int i = 0; i < maxAttempts; i++)
            {
                // Calculate a flee position with some random offset to avoid direct obstacles
                Vector3 randomizedDirection = Quaternion.Euler(0, Random.Range(-45f, 45f), 0) * fleeDirection;

                Vector3 fleePosition = transform.position + randomizedDirection * fleeDistance;

                // Check if this position is a valid point on the NavMesh
                if (NavMesh.SamplePosition(fleePosition, out NavMeshHit hit, obstacleDetectionRadius, NavMesh.AllAreas))
                {
                    enemyManager.UpdateNavAgentDestination(hit.position);

                    return this;
                }
            }
        }

        // if can't find a valid flee position, return to combat state
        return combatState;
    }

    private bool LookForMedicBag(EnemyManager enemyManager)
    {
        // cast a 2d circle to check if there are any medic bags nearby
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, medicBagDetectionRadius);

        foreach (Collider2D collider in colliders)
        {
            MedicBag medicBag = collider.GetComponent<MedicBag>();

            if (medicBag != null)
            {
                // start moving towards the medic bag
                enemyManager.UpdateNavAgentDestination(medicBag.transform.position);

                return true;
            }
        }

        return false;
    }

    public override void OnFSMStateExit(EnemyManager enemyManager, EnemyStats enemyStats)
    {
    }

    // debug draw gizmos
    private void OnDrawGizmos()
    {
        if (isDebugMode)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, medicBagDetectionRadius);
        }
    }
}