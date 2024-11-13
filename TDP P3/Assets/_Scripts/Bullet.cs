using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    protected Rigidbody2D rb;

    protected int damage;
    [SerializeField] protected GameObject impactFX;

    [Header("Collision Vars")]
    [SerializeField] protected LayerMask collisionLayers; // Set this to include only enemy layers
    protected Vector3 lastPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, float bulletSpeed)
    {
        this.damage = (int)damage;

        rb.linearVelocity = transform.up * bulletSpeed;

        // update last position to ensure collision detection works
        lastPosition = transform.position;
    }

    protected void Update()
    {
        RaycastForCollision();
        lastPosition = transform.position;
    }

    protected virtual void RaycastForCollision()
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

            lastPosition = transform.position;
            gameObject.SetActive(false);
        }

        // Debug.DrawLine(lastPosition, transform.position, Color.red);
    }
}