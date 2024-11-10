using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    private Rigidbody2D rb;

    // private DamageCollider damageCollider;

    private int damage;
    [SerializeField] private GameObject impactFX;

    [Header("Collision Vars")]
    [SerializeField] private LayerMask collisionLayers; // Set this to include only enemy layers
    private Vector3 lastPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // damageCollider = GetComponent<DamageCollider>();
    }

    public void Init(float damage, float bulletSpeed)
    {
        // damageCollider.SetDamage((int)damage);
        this.damage = (int)damage;

        rb.linearVelocity = transform.up * bulletSpeed;
    }

    private void Update()
    {
        RaycastForCollision();
        lastPosition = transform.position;
    }

    private void RaycastForCollision()
    {
        Vector3 direction = (transform.position - lastPosition).normalized;
        float distance = Vector3.Distance(transform.position, lastPosition);

        RaycastHit2D hit = Physics2D.Raycast(lastPosition, direction, distance, collisionLayers);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyStats enemyStats = hit.collider.GetComponent<EnemyStats>();
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
}