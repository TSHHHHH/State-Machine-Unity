using UnityEngine;

public class MedicCombatState : CombatState
{
    [Header("Medic Vars")]
    [SerializeField] private GameObject medicBagPrefab;

    [SerializeField] private LayerMask monitorLayer;
    [SerializeField] private float monitorHealthPercentage = 0.3f;
    [SerializeField] private float monitorDistance = 5f;

    [SerializeField] private float medicBagCooldown = 5f;
    private float medicBagTimer = 0f;


    [Header("Debug Settings")]
    [SerializeField] private bool isDebugMode = false;

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

        HandleMedicBagLogic(enemyManager);

        return this;
    }

    private void HandleMedicBagLogic(EnemyManager enemyManager)
    {
        if(medicBagTimer > 0)
        {
            medicBagTimer -= Time.deltaTime;
        }
        else
        {
            medicBagTimer = medicBagCooldown;

            // cast a sphere to check if there are any enemy allies in need of healing
            Collider2D[] colliders = Physics2D.OverlapCircleAll(enemyManager.transform.position, monitorDistance, monitorLayer);

            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Enemy"))
                {
                    EnemyStats enemyStats = collider.GetComponent<EnemyStats>();

                    if (enemyStats == null)
                        continue;

                    if (enemyStats.currentHealth <= enemyStats.maxHealth * monitorHealthPercentage)
                    {
                        // get the direction to the ally
                        Vector3 direction = collider.transform.position - enemyManager.transform.position;

                        // compute the spawn position offset from the medic
                        Vector3 spawnPosition = enemyManager.transform.position + direction.normalized * 2;

                        // create a medic bag
                        GameObject medicBagObj = Instantiate(medicBagPrefab, spawnPosition, Quaternion.identity);
                        MedicBag medicBag = medicBagObj.GetComponent<MedicBag>();

                        // compute the distance between the medic and the ally
                        float distance = Vector2.Distance(enemyManager.transform.position, collider.transform.position);

                        float speed = distance / 2f;

                        float requiredAcceleration = speed / 2f;

                        Vector2 force = direction.normalized * requiredAcceleration;

                        medicBag.Init(force);
                    }
                }
            }
        }
    }

    // debug draw
    private void OnDrawGizmos()
    {
        if (isDebugMode)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, monitorDistance);
        }
    }
}
