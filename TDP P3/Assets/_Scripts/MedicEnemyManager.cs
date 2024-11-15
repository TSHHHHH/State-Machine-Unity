using UnityEngine;

public class MedicEnemyManager : EnemyManager
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

    protected override void Update()
    {
        base.Update();

        HandleMedicBagLogic();
    }

    private void HandleMedicBagLogic()
    {
        if (medicBagTimer > 0)
        {
            medicBagTimer -= Time.deltaTime;
        }
        else
        {
            medicBagTimer = medicBagCooldown;

            // cast a sphere to check if there are any enemy allies in need of healing
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, monitorDistance, monitorLayer);

            foreach (Collider2D collider in colliders)
            {
                // skip if its itself
                if (collider.gameObject == gameObject)
                    continue;

                if (collider.CompareTag("Enemy"))
                {
                    EnemyStats enemyStats = collider.GetComponent<EnemyStats>();

                    if (enemyStats == null)
                        continue;

                    if (enemyStats.currentHealth <= enemyStats.maxHealth * monitorHealthPercentage)
                    {
                        // get the direction to the ally
                        Vector3 direction = collider.transform.position - transform.position;

                        // compute the spawn position offset from the medic
                        Vector3 spawnPosition = transform.position + direction.normalized * 1.5f;

                        // create a medic bag
                        GameObject medicBagObj = Instantiate(medicBagPrefab, spawnPosition, Quaternion.identity);
                        MedicBag medicBag = medicBagObj.GetComponent<MedicBag>();

                        // compute the distance between the medic and the ally
                        float distance = Vector2.Distance(transform.position, collider.transform.position);

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
