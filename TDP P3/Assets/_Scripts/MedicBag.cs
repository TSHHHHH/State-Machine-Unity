using UnityEngine;

public class MedicBag : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Medic Bag Vars")]
    [SerializeField] private int healAmount = 10;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector3 throwDir)
    {
        AddForce(throwDir);
    }

    private void AddForce(Vector3 throwDir)
    {
        rb.AddForce(throwDir, ForceMode2D.Impulse);

        // add a random torque to the grenade
        rb.AddTorque(Random.Range(-0.1f, 0.1f), ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerStats playerStats = collision.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                // heal the player
                playerStats.HealFixedAmount(healAmount);
            }

            Destroy(gameObject);
        }

        if (collision.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                // heal the enemy
                enemyStats.HealFixedAmount(healAmount);
            }

            Destroy(gameObject);
        }
    }
}
