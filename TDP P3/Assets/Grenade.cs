using UnityEngine;
using PrimeTween;

public class Grenade : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Grenade Vars")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private int damage = 10;
    [SerializeField] private GameObject explosionEffect;

    [Header("Debug Settings")]
    [SerializeField] private bool debugMode = false;

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

    public void Explode()
    {
        // create a sphere at the grenade's position
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                // deal damage to the player
                collider.GetComponent<PlayerStats>().TakeDamage(damage);
            }
        }

        // create the explosion effect
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // camera shake
        

        // destroy the grenade object
        Destroy(gameObject);
    }

    // debug draw the explosion radius
    private void OnDrawGizmos()
    {
        if (debugMode)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
