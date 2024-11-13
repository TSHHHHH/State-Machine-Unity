using UnityEngine;

public class EnemyBullet : Bullet
{
    protected override void RaycastForCollision()
    {
        Vector3 direction = (transform.position - lastPosition).normalized;
        float distance = Vector3.Distance(transform.position, lastPosition);

        RaycastHit2D hit = Physics2D.Raycast(lastPosition, direction, distance, collisionLayers);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                PlayerStats playerStats = hit.collider.GetComponent<PlayerStats>();
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
}
