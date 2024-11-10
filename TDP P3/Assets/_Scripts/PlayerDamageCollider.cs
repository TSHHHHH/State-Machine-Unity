using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageCollider : DamageCollider
{
    [SerializeField] private GameObject impactFX;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
            if (enemyStats != null) 
            {
                enemyStats.TakeDamage(damage);
            }
        }

        if (impactFX != null)
        {
            Instantiate(impactFX, transform.position, Quaternion.identity);
        }

        gameObject.SetActive(false);
    }
}