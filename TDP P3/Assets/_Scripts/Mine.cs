using UnityEngine;

public class Mine : MonoBehaviour
{
    [Header("Mine Settings")]
    [SerializeField] private float denoteTime = 3f;
    [SerializeField] private float explosionRadius = 5f;

    [Header("Debug Settings")]
    [SerializeField] private bool isDebugMode = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

        }
    }

    

    private void OnDrawGizmos()
    {
        if (isDebugMode)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
