using UnityEngine;

public class EnemyDamageCollider : DamageCollider
{
    [SerializeField] private GameObject impactFX;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            PlayerStats playerStats = collision.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }
        }

        if (impactFX != null)
        {
            Instantiate(impactFX, transform.position, Quaternion.identity);
        }

        gameObject.SetActive(false);
    }
}
